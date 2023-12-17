using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudioBooking.ViewModels;
using StudioBooking.Data;
using StudioBooking.Data.Models;
using StudioBooking.DTO;
using StudioBooking.Infrastructure;
using Microsoft.EntityFrameworkCore;
using static StudioBooking.Infrastructure.Enums;
using CCA.Util;
using CCAvenue;
using Cashfree;
using System.Net;
using System.Globalization;

namespace StudioBooking.Areas.User.Controllers
{
	[Area("User")]
	[Authorize]
	public class BookingController : BaseController
	{
		private readonly ILogger<BookingController> _logger;
		private readonly ICashFreeClient _cashFreeClient;
		private readonly IGoogleCalendar _googleCalendar;
		private readonly IEmailNotification _emailNotification;
		private readonly ApplicationDbContext _context;
		public BookingController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ILogger<BookingController> logger, ICashFreeClient cashFreeClient, IGoogleCalendar googleCalendar, IEmailNotification emailNotification) : base(userManager)
		{
			_logger = logger;
			_cashFreeClient = cashFreeClient;
			_googleCalendar = googleCalendar;
			_emailNotification = emailNotification;
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			var bookings = await BookingViewModel.GetCustomerBookings(_context, GetUserId());
			bookings = bookings.OrderByDescending(b => b.Booking.BookingDate).ThenBy(b => b.Booking.StartTime).ToList();
			return View(bookings);
		}

		public async Task<IActionResult> GetBooking(long id)
		{
			try
			{
				var booking = await BookingDTO.GetBookingById(_context, id);
				return Ok(new { booking });
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error for Booking ID: {id}", ex);
				return BadRequest(ex.Message);
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetCancelModal(long id)
		{
			try
			{
				var booking = await _context.Bookings.Include(b => b.Customer).Include(b => b.ServicePrice.Service).Include(b => b.ServicePrice.Category)
					.Include(b => b.Transactions).FirstOrDefaultAsync(b => b.Id == id && !b.IsDelete && b.IsActive);

				if (booking.BookingStatus == (int)BookingStatus.OnHold || booking.BookingStatus == (int)BookingStatus.ReScheduled) throw new Exception("Booking is not eligible for cancellation");

				var totalPaid = booking.Transactions.Where(t => t.Status == (int)TransactionStatus.Success).Sum(t => t.Amount);
				var model = new CancelBookingDTO
				{
					Id = booking.Id,
					BookingId = booking.Id.ToString(Defaults.BookingPrefix),
					BookingStatus = (BookingStatus)booking.BookingStatus,
					BookingDate = booking.BookingDate.ToString(Defaults.DefaultDateFormat),
					BookingEndDate = booking.BookingEndDate.ToString(Defaults.DefaultDateFormat),
					StartTime = booking.StartTime,
					EndTime = booking.EndTime,
					Total = totalPaid,
					RefundAmount = (booking.Total - totalPaid) == 0 ? booking.Total / 2 : 0,
					ServiceName = booking.ServicePrice.Service.Name,
					CategoryName = booking.ServicePrice.Category.Name
				};
				return PartialView("_CancelRequestModal", model);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		public IActionResult CashFreePayment(string paymentId)
		{
			return View(new PaymentGatewayResponse { PaymentSessionId = paymentId });
		}

		public IActionResult CCAvenuePayment(string encRequest)
		{
			return View(new CCAvenuePymentRequestDTO { encRequest = encRequest, access_code = CCAvenueConfig.AccessCode });
		}

		public async Task<IActionResult> PayNow(long id, long scheduleRequestId = 0)
		{
			var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);
			if (booking.PaymentStatus == (int)PaymentStatus.FullPayment && scheduleRequestId == 0) throw new Exception("Booking payment is completed");
			var customer = await CustomerDTO.GetCustomer(_context, GetUserId());
			if (booking.CustomerId != customer.Id) throw new InvalidOperationException("Booking not exists for customer");
			var transactions = await _context.Transactions.Where(c => c.BookingId == booking.Id && c.Status == (int)TransactionStatus.Success).ToListAsync();
			var servicePrice = await ServicePriceDTO.GetServicePrice(_context, booking.ServicePriceId);
			var totalPaid = transactions.Sum(t => t.Amount);
			var customerAddress = await CustomerAddressDTO.GetAddress(_context, booking.BillingAddressId ?? 0);
			if (customerAddress.Id <= 0)
			{
				var addressses = await CustomerAddressDTO.GetAddressByCustomerId(_context, customer.Id);
				customerAddress = addressses.FirstOrDefault(s => s.IsDefault);
			}
			var bookingViewModel = new BookingViewModel
			{
				Addons = booking.IsAddonRequested ? await _context.Addons.Where(a => a.BookingId == booking.Id && a.IsActive && a.AdjustmentType == (int)AdjustmentType.Add).Select(s => new AddOnDTO
				{
					Id = s.Id,
					Name = s.Name,
					Description = s.Description
				}).ToListAsync() : new List<AddOnDTO>(),
				ServicePrice = servicePrice,
				Customer = customer,
				CustomerAddresses = new List<CustomerAddressDTO> { customerAddress },
				Wallets = customer.Id > 0 ? await WalletDTO.GetCustomerWallets(_context, customer.Id) : new List<WalletDTO>()
			};

			if (transactions.Any(t => t.PaymentMode == (int)Enums.PaymentMode.Online || t.PaymentMode == (int)(Enums.PaymentMode.Wallet | Enums.PaymentMode.Online)))
			{
				totalPaid += await WalletDTO.GetBookingPaymentsFromWalletAsync(_context, transactions);
			}

			if (scheduleRequestId != 0)
			{
				var scheduleRequest = await _context.ScheduleRequests.FirstOrDefaultAsync(s => s.Id == scheduleRequestId);
				if (scheduleRequest != null)
				{
					var endTime = scheduleRequest.RequestEndTime ?? DateTime.MinValue;
					var startTime = scheduleRequest.RequestStartTime ?? DateTime.MinValue;
					var totalHours = (endTime - startTime).TotalHours;
					bookingViewModel.ScheduleRequestId = scheduleRequest.Id;
					bookingViewModel.Booking = new BookingDTO
					{
						Id = booking.Id,
						AdvancePaid = totalPaid,
						BookingDate = scheduleRequest.RequestDate.ToString(),
						BookingEndDate = scheduleRequest.RequestEndDate.ToString(),
						RatePerHour = booking.RatePerHour,
						StartTime = TimeOnly.FromDateTime(startTime).ToString(),
						EndTime = TimeOnly.FromDateTime(endTime).ToString(),
						Total = totalHours * (double)booking.RatePerHour, //booking.Total,
					};
				}
			}
			else
			{
				bookingViewModel.Booking = new BookingDTO
				{
					Id = booking.Id,
					AdvancePaid = totalPaid,
					BookingDate = booking.BookingDate.ToString(Defaults.DefaultDateFormat),
					BookingEndDate = booking.BookingEndDate.ToString(Defaults.DefaultDateFormat),
					RatePerHour = booking.RatePerHour,
					StartTime = booking.StartTime,
					EndTime = booking.EndTime,
					Total = booking.Total,
				};
			}
			return View(bookingViewModel);
		}

		public IActionResult Failed(TransactionDTO transaction)
		{
			return View(transaction);
		}

		[AllowAnonymous]
		public async Task<IActionResult> ThankYou(string TXNID, string order_id)
		{
			var transaction = await _context.Transactions.FirstOrDefaultAsync(c => c.Id == Convert.ToInt64(TXNID));
			try
			{
				if (transaction.Status == (int)TransactionStatus.Pending)
				{
					var booking = await _context.Bookings.Include(t => t.Transactions).FirstOrDefaultAsync(c => c.Id == transaction.BookingId);
					var pendingAmount = booking.Total - booking.Transactions.Where(t => t.Status == (int)TransactionStatus.Success).Sum(b => b.Amount);
					if (booking.Transactions.Any(t => t.PaymentMode == (int)Enums.PaymentMode.Online || t.PaymentMode == (int)(Enums.PaymentMode.Wallet | Enums.PaymentMode.Online)))
					{
						pendingAmount += await WalletDTO.GetBookingPaymentsFromWalletAsync(_context, booking.Transactions.ToList());
					}
					if (!Equals(order_id, Enums.PaymentMode.Offline))
					{
						var paymentGatewayResponse = await _context.PaymentGatewayResponses.FirstOrDefaultAsync(c => c.TransactionId == transaction.Id);
						if (transaction.PaymentProvider == (int)PaymentProvider.CashFree)
						{
							var paymentOrder = await _cashFreeClient.GetAsync(PaymentGatewayAPIs.GetOrder + order_id);
							if (paymentOrder.IsSuccessStatusCode)
							{
								var response = await paymentOrder.Content.ReadAsStringAsync();
								var orderResponse = Converter.ConvertJsonToObject<OrderResponse>(response);
								booking.BookingStatus = (Cashfree.OrderStatus)Enum.Parse(typeof(Cashfree.OrderStatus), orderResponse.order_status) == Cashfree.OrderStatus.PAID ? (int)BookingStatus.Booked : (int)BookingStatus.Pending;
								booking.PaymentStatus = pendingAmount > 0 ? (int)PaymentStatus.Advance : (int)PaymentStatus.FullPayment;
								transaction.Status = (Cashfree.OrderStatus)Enum.Parse(typeof(Cashfree.OrderStatus), orderResponse.order_status) == Cashfree.OrderStatus.PAID ? (int)TransactionStatus.Success : (int)TransactionStatus.Failed;
								paymentGatewayResponse.OrderStatus = (int)Enum.Parse(typeof(Cashfree.OrderStatus), orderResponse.order_status);
							}
							else
							{
								booking.BookingStatus = pendingAmount == booking.Total ? (int)BookingStatus.Failed : booking.BookingStatus;
								booking.PaymentStatus = pendingAmount == booking.Total ? (int)PaymentStatus.Pending : pendingAmount > 0 ? (int)PaymentStatus.Advance : (int)PaymentStatus.FullPayment;
								transaction.Status = (int)TransactionStatus.Failed;
							}
						}
						if (transaction.PaymentProvider == (int)PaymentProvider.CCAvenue)
						{
							var paymentResponse = PaymentResponse.GetPaymentResponse(Request.Form["encResp"], CCAvenueConfig.WorkingKey);
							_logger.LogInformation("Inside Thank You, PaymentProvider.CCAvenue: " + Converter.ConvertObjectToJsonString(paymentResponse));
							if (paymentResponse.order_status == "Success")
							{
								booking.BookingStatus = (CCAvenue.OrderStatus)Enum.Parse(typeof(CCAvenue.OrderStatus), paymentResponse.order_status) == CCAvenue.OrderStatus.Success ? (int)BookingStatus.Booked : (int)BookingStatus.Pending;
								booking.PaymentStatus = (pendingAmount - transaction.Amount) > 0 ? (int)PaymentStatus.Advance : (int)PaymentStatus.FullPayment;
								transaction.Status = (CCAvenue.OrderStatus)Enum.Parse(typeof(CCAvenue.OrderStatus), paymentResponse.order_status) == CCAvenue.OrderStatus.Success ? (int)TransactionStatus.Success : (int)TransactionStatus.Failed;
								paymentGatewayResponse.OrderId = paymentResponse.order_id;
								paymentGatewayResponse.BankRefNo = paymentResponse.bank_ref_no;
								paymentGatewayResponse.PaymentSessionId = paymentResponse.tracking_id;
								paymentGatewayResponse.FailureMessage = paymentResponse.failure_message;
								paymentGatewayResponse.PaymentMode = paymentResponse.payment_mode;
								paymentGatewayResponse.CardName = paymentResponse.card_name;
								paymentGatewayResponse.StatusCode = paymentResponse.status_code;
								paymentGatewayResponse.StatusMessage = paymentResponse.status_message;
								paymentGatewayResponse.OrderCurrency = paymentResponse.currency;
								paymentGatewayResponse.OrderAmount = paymentResponse.amount;
								paymentGatewayResponse.mer_amount = paymentResponse.mer_amount;
								paymentGatewayResponse.trans_date = paymentResponse.trans_date;
								paymentGatewayResponse.discount_value = paymentResponse.discount_value;
								paymentGatewayResponse.eci_value = paymentResponse.eci_value;
								paymentGatewayResponse.response_code = paymentResponse.response_code;
								paymentGatewayResponse.OrderStatus = (int)Enum.Parse(typeof(CCAvenue.OrderStatus), paymentResponse.order_status);

								if (!string.IsNullOrEmpty(paymentResponse.merchant_param2))
								{
									if (int.Parse(paymentResponse.merchant_param2) > 0)
									{
										paymentGatewayResponse.OrderNote = paymentResponse.merchant_param2;
										var request = await _context.ScheduleRequests.FirstOrDefaultAsync(r => r.Id == int.Parse(paymentResponse.merchant_param2)) ?? new ScheduleRequest();
										var newTotalHours = ((request.RequestEndTime ?? DateTime.MinValue) - (request.RequestStartTime ?? DateTime.MinValue)).TotalHours;
										request.RequestStatus = (int)RequestStatus.Completed;
										booking.BookingDate = request.RequestDate ?? DateTime.MinValue;
										booking.StartTime = TimeOnly.FromDateTime(request.RequestStartTime ?? DateTime.MinValue).ToString();
										booking.EndTime = TimeOnly.FromDateTime(request.RequestEndTime ?? DateTime.MinValue).ToString();
										booking.Total = newTotalHours * (double)booking.RatePerHour;
										booking.PaymentStatus = (pendingAmount - transaction.Amount) > 0 ? (int)PaymentStatus.Advance : (int)PaymentStatus.FullPayment;
										booking.BookingStatus = (int)BookingStatus.ReScheduled;
										await _context.SaveChangesAsync();
										if (string.IsNullOrEmpty(booking.CalenderEventId))
										{
											try
											{
												await EventDTO.AddCalenderEvent(_context, _googleCalendar, booking);
												await _context.SaveChangesAsync();
											}
											catch (Exception ex)
											{
												_logger.LogError("Error at adding calender event ScheduleChangeRequest: " + ex.Message, ex);
											}
										}
										else
										{
											await EventDTO.UpdateCalenderEvent(_context, _googleCalendar, booking);
										}

										//Reschedule/Cancellation
										var serviceDetail = await ServicePriceDTO.GetServicePrice(_context, booking.ServicePriceId);
										var customer = await _context.Customers.Include(c => c.User).FirstOrDefaultAsync(u => u.Id == booking.CustomerId);
										var bookingDetail = await BookingDTO.GetBookingDetailById(_context, booking.Id);
										bookingDetail.ServiceName = serviceDetail.ServiceName;
										bookingDetail.CategoryName = serviceDetail.CategoryName;
										bookingDetail.ServiceTitle = Defaults.GetServiceTitle(serviceDetail.ServiceName);
										var websiteSetting = await WebsiteSettingDTO.GetWebsiteSettingAsync(_context);
										await _emailNotification.SendReScheduleCancellationNotification(request, bookingDetail, customer?.User ?? new ApplicationUser(), websiteSetting);
									}
								}
							}
							else
							{
								booking.BookingStatus = pendingAmount == booking.Total ? (int)BookingStatus.Failed : booking.BookingStatus;
								booking.PaymentStatus = pendingAmount == booking.Total ? (int)PaymentStatus.Pending : pendingAmount > 0 ? (int)PaymentStatus.Advance : (int)PaymentStatus.FullPayment;
								transaction.Status = (int)TransactionStatus.Failed;
							}
						}
						transaction.ModifiedBy = transaction.CreatedBy;
						transaction.ModifiedDate = Defaults.GetDateTime();
						booking.ModifiedBy = transaction.ModifiedBy;
						booking.ModifiedDate = Defaults.GetDateTime();
					}
					else
					{
						booking.BookingStatus = (int)BookingStatus.WaitingForApproval;
					}
					if (booking.BookingStatus == (int)BookingStatus.Booked && string.IsNullOrEmpty(booking.CalenderEventId))
					{
						try
						{
							await EventDTO.AddCalenderEvent(_context, _googleCalendar, booking);
							//ConfirmBooking
							var websiteSetting = await WebsiteSettingDTO.GetWebsiteSettingAsync(_context);
							var bookingDteail = await BookingDTO.GetBookingDetailById(_context, booking.Id);
							var customer = await _context.Customers.Include(c => c.User).FirstOrDefaultAsync(u => u.Id == booking.CustomerId);
							var serviceDetail = await ServicePriceDTO.GetServicePrice(_context, booking.ServicePriceId);
							bookingDteail.ServiceName = serviceDetail.ServiceName;
							bookingDteail.CategoryName = serviceDetail.CategoryName;
							bookingDteail.ServiceTitle = Defaults.GetServiceTitle(serviceDetail.ServiceName);
							await _emailNotification.SendBookingNotification(bookingDteail, customer.User, websiteSetting);
						}
						catch (Exception ex)
						{
							_logger.LogError("Error at adding calender event User>Booking>ThankYou: " + ex.Message, ex);
						}

					}
					await _context.SaveChangesAsync();
				}
				if (transaction.Status == (int)TransactionStatus.Failed) throw new InvalidOperationException("Payment failed by payment gateway.");
			}
			catch (Exception ex)
			{
				_logger.LogError("Error at User Booking Thankyou: " + ex.Message, ex);
				return RedirectToAction("Failed", new TransactionDTO { Id = transaction.Id, BookingId = transaction.BookingId });
			}
			return View(new TransactionDTO { Id = transaction.Id, BookingId = transaction.BookingId });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CompletePayment(BookingViewModel model)
		{
			var bookingDetail = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == model.Booking.Id);
			var paymentGatewayResponse = new PaymentGatewayResponse();
			CustomerAddressDTO? customerAddress = null;
			var strEncRequest = string.Empty;
			var responseUrl = string.Empty;
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
			var customer = await Customer.GetCustomerByUserId(_context, GetUserId());
			if (model.DifferentBillingAddress)
				customerAddress = await CustomerAddressDTO.SaveAddressAsync(_context, customer.Id, model.CustomerAddress ?? new CustomerAddressDTO());
			else
				customerAddress = await CustomerAddressDTO.GetAddress(_context, model.Booking.BillingAddressId ?? 0);
			await _context.SaveChangesAsync();
			using var transactionScope = _context.Database.BeginTransaction();
			bookingDetail.BillingAddressId = customerAddress.Id;
			//bookingDetail.PaymentStatus = (int)model.Booking.PaymentStatus;
			var transaction = await SetTransactionAsync(bookingDetail, (int)model.Booking.PaymentStatus);
			transaction.PaymentProvider = (int)model.Booking.PaymentProvider;
			await _context.Transactions.AddAsync(transaction);
			await _context.SaveChangesAsync();
			if (model.Booking.WalletType != WalletType.None)
			{
				var customerWallet = await _context.Wallets.FirstOrDefaultAsync(s => s.CustomerId == bookingDetail.CustomerId && s.WalletType == (int)model.Booking.WalletType);
				if (customerWallet != null)
				{
					var debitAmount = (bookingDetail.Total > (customerWallet.BalanceAmount ?? 0)) ? customerWallet.BalanceAmount : bookingDetail.Total;
					transaction.Amount = debitAmount <= transaction.Amount ? model.Booking.PaymentStatus == PaymentStatus.Advance ? Math.Round((bookingDetail.Total - customerWallet.BalanceAmount ?? 0) / 2, 2) : Math.Round((bookingDetail.Total - customerWallet.BalanceAmount ?? 0), 2) : 0;
					var walletTransaction = new WalletTransactionDTO { Amount = debitAmount, Description = transaction.Id.ToString(Defaults.TransactionPrefix), TransactionType = TransactionType.Debit, ReferenceId = bookingDetail.Id.ToString(Defaults.BookingPrefix), Mode = (WalletType)customerWallet.Mode };
					await WalletDTO.AddUpdateWallet(_logger, _context, customerWallet, walletTransaction, user.Id);
					transaction.PaymentMode = transaction.Amount > 0 ? (int)(Enums.PaymentMode.Wallet | Enums.PaymentMode.Online) : (int)Enums.PaymentMode.Wallet;
				}
			}
			else
			{
				transaction.PaymentMode = (int)Enums.PaymentMode.Online;
			}
			await _context.SaveChangesAsync();
			if (transaction.PaymentMode == (int)Enums.PaymentMode.Online || transaction.PaymentMode == (int)(Enums.PaymentMode.Wallet | Enums.PaymentMode.Online))
			{
				if (model.Booking.PaymentProvider == PaymentProvider.CashFree)
				{
					paymentGatewayResponse = await InitiateCashFreePaymentRequestAsync(bookingDetail, transaction, user);
					responseUrl = Url.Action("CashFreePayment", "Booking", new { paymentId = paymentGatewayResponse.PaymentSessionId }, Request.Scheme);
				}
				if (model.Booking.PaymentProvider == PaymentProvider.CCAvenue)
				{
					var order = GetOrderRequest(bookingDetail, transaction, user);
					var ccaCrypto = new CCACrypto();
					string ccaRequest = "language=en&merchant_id=" + CCAvenueConfig.MerchantId + "&order_id=" + bookingDetail.Id.ToString(Defaults.BookingPrefix) + "&amount=" + 1 + "&currency=" + Defaults.DefaultCurrency + "&redirect_url=" + Url.Action("ThankYou", "Booking", new { TXNID = transaction.Id }, Request.Scheme) + "&cancel_url=" + Url.Action("ThankYou", "Booking", new { TXNID = transaction.Id }, Request.Scheme) + "&billing_name=" + customerAddress.Name +
					"&customer_identifier=" + bookingDetail.CustomerId.ToString() + "&merchant_param1=" + transaction.Id.ToString(Defaults.TransactionPrefix) +
					"&billing_address=" + CustomerAddressDTO.GetAddress(customerAddress) + "&billing_city=" + customerAddress.City + "&billing_state=" + customerAddress.State + "&billing_zip=" + customerAddress.PinCode + "&billing_country=India&billing_email=" + user.Email + "&billing_tel=" + user.PhoneNumber + "&merchant_param2=" + model.ScheduleRequestId;
					strEncRequest = ccaCrypto.Encrypt(ccaRequest, CCAvenueConfig.WorkingKey);
					paymentGatewayResponse = InitiateCCAvenuePaymentRequest(bookingDetail, transaction);
					responseUrl = Url.Action("CCAvenuePayment", "Booking", new { encRequest = strEncRequest }, Request.Scheme);
				}
				await _context.PaymentGatewayResponses.AddAsync(paymentGatewayResponse);
				await _context.SaveChangesAsync();
			}
			else
			{
				bookingDetail.BookingStatus = (int)BookingStatus.Booked;
				transaction.Status = (int)TransactionStatus.Success;
			}
			if (bookingDetail.BookingStatus == (int)BookingStatus.Booked && string.IsNullOrEmpty(bookingDetail.CalenderEventId))
			{
				try
				{
					var serviceDetail = await ServicePriceDTO.GetServicePrice(_context, bookingDetail.ServicePriceId);
					var calenderEvent = new EventDTO
					{
						Date = DateOnly.FromDateTime(bookingDetail.BookingDate),
						EndDate = DateOnly.FromDateTime(bookingDetail.BookingEndDate),
						StartTime = TimeOnly.FromDateTime(Convert.ToDateTime(bookingDetail.StartTime)),
						EndTime = TimeOnly.FromDateTime(Convert.ToDateTime(bookingDetail.EndTime)),
						Title = customer.Name,
						Description = bookingDetail.Id.ToString(Defaults.BookingPrefix) + " - " + serviceDetail.ServiceName + " - " + serviceDetail.CategoryName,
						Email = user.Email,
						CalendarName = serviceDetail.CalenderName,
						Studio = serviceDetail.CategoryName,
						ColorId = serviceDetail.EventColorId ?? "1"
					};
					bookingDetail.CalenderEventId = await _googleCalendar.AddCalenderEventAsync(calenderEvent);
					await _context.SaveChangesAsync();
				}
				catch (Exception ex)
				{
					_logger.LogError("Error at adding calender event: " + ex.Message, ex);
				}
			}
			await transactionScope.CommitAsync();
			responseUrl = responseUrl != string.Empty ? responseUrl : Url.Action("ThankYou", "Booking", new { TXNID = transaction.Id, payment_id = model.Booking.PaymentMode }, Request.Scheme);
			return Redirect(responseUrl);
		}

		public async Task<IActionResult> Transactions()
		{
			var transactions = await TransactionDTO.GetUserTransactions(_context, GetUserId());
			return View(transactions);
		}

		[HttpPost]
		public async Task<IActionResult> ScheduleChangeRequest([FromBody] ScheduleRequestDTO modal)
		{
			try
			{
				var booking = await _context.Bookings.Include(b => b.Transactions).FirstOrDefaultAsync(b => b.Id == modal.BookingId) ?? throw new Exception("Invalid booking id");
				if (modal.RequestType == RequestType.Cancel)
				{
					var request = new ScheduleRequest
					{
						BookingId = booking.Id,
						BookingDate = booking.BookingDate,
						BookingEndDate = booking.BookingEndDate,
						StartTime = booking.StartTime,
						EndTime = booking.EndTime,
						RequestStatus = booking.BookingStatus == (int)BookingStatus.WaitingForApproval ? (int)RequestStatus.Completed : (int)RequestStatus.Pending,
						RequestType = (int)modal.RequestType,
						Description = modal.Description,
						PaymentProvider = modal.PaymentProvider != null ? (int)modal.PaymentProvider : null,
						Account = modal.Account,
						Name = modal.Name,
						IFSC = modal.IFSC,
						IsActive = true,
						CreatedBy = GetUserId(),
						CreatedDate = Defaults.GetDateTime(),
						ModifiedBy = GetUserId(),
						ModifiedDate = Defaults.GetDateTime()
					};
					booking.BookingStatus = booking.BookingStatus == (int)BookingStatus.WaitingForApproval ? (int)BookingStatus.Cancelled : (int)BookingStatus.WaitingForCancellation;
					booking.ModifiedDate = Defaults.GetDateTime();
					booking.ModifiedBy = GetUserId();
					await _context.ScheduleRequests.AddAsync(request);
					await _context.SaveChangesAsync();
					if (booking.BookingStatus == (int)BookingStatus.Cancelled)
					{
						//Reschedule/Cancellation
						var serviceDetail = await ServicePriceDTO.GetServicePrice(_context, booking.ServicePriceId);
						var bookingDetail = await BookingDTO.GetBookingDetailById(_context, booking.Id);
						var customer = await _context.Customers.Include(c => c.User).AsNoTracking().FirstOrDefaultAsync(u => u.Id == booking.CustomerId);
						bookingDetail.ServiceName = serviceDetail.ServiceName;
						bookingDetail.CategoryName = serviceDetail.CategoryName;
						bookingDetail.ServiceTitle = Defaults.GetServiceTitle(serviceDetail.ServiceName);
						var websiteSetting = await WebsiteSettingDTO.GetWebsiteSettingAsync(_context);
						await _emailNotification.SendReScheduleCancellationNotification(request, bookingDetail, customer?.User ?? new ApplicationUser(), websiteSetting);
					}
				}
				else
				{
					var request = new ScheduleRequest
					{
						BookingId = booking.Id,
						BookingDate = booking.BookingDate,
						BookingEndDate = booking.BookingEndDate,
						RequestEndDate = DateTime.Parse(modal.RequestEndDate),
						RequestDate = DateTime.Parse(modal.RequestDate),
						StartTime = booking.StartTime,
						EndTime = booking.EndTime,
						RequestStartTime = DateTime.Parse(modal.RequestStartTime),
						RequestEndTime = DateTime.Parse(modal.RequestEndTime),
						RequestStatus = (int)RequestStatus.Pending,
						RequestType = (int)modal.RequestType,
						Description = modal.Description,
						IsActive = true,
						CreatedBy = GetUserId(),
						CreatedDate = Defaults.GetDateTime(),
						ModifiedBy = GetUserId(),
						ModifiedDate = Defaults.GetDateTime()
					};
					await _context.ScheduleRequests.AddAsync(request);
					await _context.SaveChangesAsync();
					modal.Id = request.Id;
					//var newTotalHours = ((modal.RequestEndTime ?? DateTime.MinValue) - (modal.RequestStartTime ?? DateTime.MinValue)).TotalHours;
					//var oldTotalHours = (DateTime.Parse(booking.EndTime) - DateTime.Parse(booking.StartTime)).TotalHours;
					//var newTotalHours = (modal.RequestEndDate?.Add(TimeSpan.Parse(modal.RequestEndTime)) - modal.RequestDate?.Add(TimeSpan.Parse(modal?.RequestStartTime)))?.TotalHours;

					////Commented By Jainam
					//var newTotalHours = (DateOnly.Parse(modal.RequestEndDate).ToDateTime(TimeOnly.Parse(modal.RequestEndTime)) - DateOnly.Parse(modal.RequestDate).ToDateTime(TimeOnly.Parse(modal.RequestStartTime))).TotalHours;
					//var oldTotalHours = (booking.BookingEndDate.Add(TimeSpan.Parse(booking.EndTime)) - booking.BookingDate.Add(TimeSpan.Parse(booking.StartTime))).TotalHours;

					var newTotalHours = (DateTime.ParseExact(modal.BookingEndDate + " " + modal.EndTime, Defaults.DefaultDateTime24Format, CultureInfo.InvariantCulture) - DateTime.ParseExact(modal.BookingDate + " " + modal.StartTime, Defaults.DefaultDateTime24Format, CultureInfo.InvariantCulture)).TotalHours;
					var oldTotalHours = (DateTime.ParseExact(booking.BookingEndDate + " " + booking.EndTime, Defaults.DefaultDateTime24Format, CultureInfo.InvariantCulture) - DateTime.ParseExact(booking.BookingDate + " " + booking.StartTime, Defaults.DefaultDateTime24Format, CultureInfo.InvariantCulture)).TotalHours;

					if (newTotalHours <= oldTotalHours)
					{
						var pendingAmount = booking.Total - booking.Transactions.Where(t => t.Status == (int)TransactionStatus.Success).Sum(b => b.Amount);
						if (pendingAmount <= 0)
						{
							request.RequestStatus = (int)RequestStatus.Completed;
							booking.BookingDate = request.RequestDate ?? DateTime.MinValue;
							booking.BookingEndDate = request.RequestEndDate ?? DateTime.MinValue;
							booking.StartTime = TimeOnly.FromDateTime(request.RequestStartTime ?? DateTime.MinValue).ToString();
							booking.EndTime = TimeOnly.FromDateTime(request.RequestEndTime ?? DateTime.MinValue).ToString();
							booking.Total = newTotalHours * (double)booking.RatePerHour;
							booking.BookingStatus = (int)BookingStatus.ReScheduled;
							booking.PaymentStatus = pendingAmount > 0 ? (int)PaymentStatus.Advance : (int)PaymentStatus.FullPayment;
							await _context.SaveChangesAsync();
							if (string.IsNullOrEmpty(booking.CalenderEventId))
							{
								try
								{
									await EventDTO.AddCalenderEvent(_context, _googleCalendar, booking);
									await _context.SaveChangesAsync();
								}
								catch (Exception ex)
								{
									_logger.LogError("Error at adding calender event ScheduleChangeRequest: " + ex.Message, ex);
								}
							}
							else
							{
								await EventDTO.UpdateCalenderEvent(_context, _googleCalendar, booking);
							}

							//Reschedule/Cancellation
							var serviceDetail = await ServicePriceDTO.GetServicePrice(_context, booking.ServicePriceId);
							var customer = await _context.Customers.Include(c => c.User).FirstOrDefaultAsync(u => u.Id == booking.CustomerId);
							var bookingDetail = await BookingDTO.GetBookingDetailById(_context, booking.Id);
							bookingDetail.ServiceName = serviceDetail.ServiceName;
							bookingDetail.CategoryName = serviceDetail.CategoryName;
							bookingDetail.ServiceTitle = Defaults.GetServiceTitle(serviceDetail.ServiceName);
							var websiteSetting = await WebsiteSettingDTO.GetWebsiteSettingAsync(_context);
							await _emailNotification.SendReScheduleCancellationNotification(request, bookingDetail, customer?.User ?? new ApplicationUser(), websiteSetting);
						}
						else
							modal.RedirectToPayment = pendingAmount > 0;
					}
					else
						modal.RedirectToPayment = newTotalHours > oldTotalHours;
					//return RedirectToAction("PayNow", new { id = modal.BookingId, scheduleRequestId = request.Id });
				}
			}
			catch (Exception ex)
			{
				return Ok(new { data = modal, result = false, error = ex.Message });
			}
			return Ok(new { data = modal, result = true });
		}
		private async Task<Transaction> SetTransactionAsync(Booking booking, int paymentType)
		{
			var transactions = await _context.Transactions.Where(c => c.BookingId == booking.Id && c.Status == (int)TransactionStatus.Success).ToListAsync();
			DateTime endTime, startTime = DateTime.MinValue;
			double totalHours = 0, pendingPayment = 0, advancePaid = 0;
			endTime = DateTime.Parse(booking.EndTime);
			startTime = DateTime.Parse(booking.StartTime);
			totalHours = (endTime - startTime).TotalHours;
			advancePaid = transactions.Sum(t => t.Amount);
			if (transactions.Any(t => t.PaymentMode == (int)Enums.PaymentMode.Online || t.PaymentMode == (int)(Enums.PaymentMode.Wallet | Enums.PaymentMode.Online)))
			{
				advancePaid += await WalletDTO.GetBookingPaymentsFromWalletAsync(_context, transactions);
			}
			pendingPayment = advancePaid > 0 ? (booking.Total - advancePaid) : booking.Total;

			return new Transaction
			{
				BookingId = booking.Id,
				PaymentType = booking.PaymentStatus,
				Status = (int)TransactionStatus.Pending,
				TransactionType = (int)TransactionType.Debit,
				Amount = paymentType == (int)Enums.PaymentType.Advance ? totalHours * (float)(booking.RatePerHour / 2) : pendingPayment,
				CustomerId = booking.CustomerId,
				IsActive = true,
				CreatedBy = GetUserId(),
				CreatedDate = Defaults.GetDateTime(),
				ModifiedBy = GetUserId(),
				ModifiedDate = Defaults.GetDateTime()
			};
		}

		private OrderRequest GetOrderRequest(Booking bookingDetail, Transaction transaction, ApplicationUser user)
		{
			return new OrderRequest
			{
				order_id = bookingDetail.Id.ToString(Defaults.BookingPrefix),
				order_amount = transaction.Amount,
				order_currency = Defaults.DefaultCurrency,
				order_expiry_time = Defaults.GetDateTime().AddHours(1),
				order_meta = new OrderMeta
				{
					return_url = Url.Action("ThankYou", "Booking", new { TXNID = transaction.Id }, Request.Scheme)
				},
				customer_details = new CustomerDetail
				{
					customer_id = bookingDetail.CustomerId.ToString(),
					customer_email = user.Email,
					customer_phone = user.PhoneNumber,
					customer_name = user.FirstName + " " + user.LastName,
				}
			};
		}

		private async Task<PaymentGatewayResponse> InitiateCashFreePaymentRequestAsync(Booking bookingDetail, Transaction transaction, ApplicationUser user)
		{
			var order = GetOrderRequest(bookingDetail, transaction, user);
			order.order_meta.return_url = order.order_meta.return_url + "&order_id={order_id}";
			var result = await _cashFreeClient.PostAsync(order, PaymentGatewayAPIs.Order);
			if (result.StatusCode == HttpStatusCode.OK)
			{
				var response = await result.Content.ReadAsStringAsync();
				var orderResponse = Converter.ConvertJsonToObject<OrderResponse>(response);
				return new PaymentGatewayResponse
				{
					TransactionId = transaction.Id,
					BookingId = bookingDetail.Id,
					OrderId = orderResponse.cf_order_id.ToString(),
					OrderAmount = orderResponse.order_amount.ToString(),
					OrderCurrency = orderResponse.order_currency,
					OrderNote = orderResponse.order_note,
					OrderExpiryTime = orderResponse.order_expiry_time,
					PaymentSessionId = orderResponse.payment_session_id,
					OrderStatus = (int)Enum.Parse(typeof(Cashfree.OrderStatus), orderResponse.order_status),
					PaymentUrl = orderResponse.payments.url,
					RefundUrl = orderResponse.refunds.url,
					SettlementUrl = orderResponse.settlements.url,
					IsActive = true,
					CreatedBy = GetUserId(),
					CreatedDate = Defaults.GetDateTime(),
					ModifiedBy = GetUserId(),
					ModifiedDate = Defaults.GetDateTime()
				};
			}
			return null;
		}

		private PaymentGatewayResponse InitiateCCAvenuePaymentRequest(Booking booking, Transaction transaction)
		{

			return new PaymentGatewayResponse
			{
				TransactionId = transaction.Id,
				BookingId = booking.Id,
				OrderId = booking.Id.ToString(Defaults.BookingPrefix),
				OrderAmount = transaction.Amount.ToString(),
				OrderCurrency = Defaults.DefaultCurrency,
				IsActive = true,
				CreatedBy = GetUserId(),
				CreatedDate = Defaults.GetDateTime(),
				ModifiedBy = GetUserId(),
				ModifiedDate = Defaults.GetDateTime()
			};
		}
	}
}
