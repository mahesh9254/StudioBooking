using Cashfree;
using CCA.Util;
using CCAvenue;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using StudioBooking.Data;
using StudioBooking.Data.Models;
using StudioBooking.DTO;
using StudioBooking.Infrastructure;
using StudioBooking.ViewModels;
using System.Net;
using static StudioBooking.Infrastructure.Enums;

namespace StudioBooking.Controllers
{
    [Authorize]
    public class BookingController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ICashFreeClient _cashFreeClient;
        private readonly IGoogleCalendar _googleCalendar;
        private readonly IEmailNotification _emailNotification;
        private readonly ILogger<BookingController> _logger;
        public BookingController(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, ICashFreeClient cashFreeClient, IGoogleCalendar googleCalendar, IEmailNotification emailNotification, ILogger<BookingController> logger) : base(userManager)
        {
            _context = applicationDbContext;
            _cashFreeClient = cashFreeClient;
            _googleCalendar = googleCalendar;
            _emailNotification = emailNotification;
            _logger = logger;
        }

        public IActionResult ComingSoon()
        {
            return View();
        }

        public async Task<IActionResult> PaymentReciept(long id)
        {
            var bookings = await PaymentReceiptViewModel.GetPaymentReceipt(_context, id);
            //return new ViewAsPdf("PaymentReciept",bookings);
            return View(bookings);
        }

        public async Task<IActionResult> Index()
        {
            var bookingViewModel = new BookingViewModel
            {
                ServicePrices = await ServicePriceDTO.GetServicePrices(_context)
            };
            return View(bookingViewModel);
        }

        public async Task<IActionResult> Checkout()
        {
            var activeCart = Common.GetCookie(Request, "_crt");
            if (activeCart == null)
                return RedirectToAction("Index", "Service");
            var cart = Converter.ConvertJsonToObject<CartDTO>(Encryption.Decrypt(activeCart));
            var servicePrice = await ServicePriceDTO.GetServicePrice(_context, cart.ServicePriceId);
            if (servicePrice.EnableTwoStepBooking)
                return RedirectToAction("TwoStepBooking", "Booking");

            var customer = await CustomerDTO.GetCustomer(_context, GetUserId());
            var bookingViewModel = new BookingViewModel
            {
                Cart = cart,
                ServicePrice = servicePrice,
                Customer = customer,
                CustomerAddresses = await CustomerAddressDTO.GetAddressByCustomerId(_context, customer.Id),
                Wallets = customer.Id > 0 ? await WalletDTO.GetCustomerWallets(_context, customer.Id) : new List<WalletDTO>(),
            };            
            return View(bookingViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookNow(BookingViewModel model)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.Id == model.Cart.Id && !(c.IsCheckedOut ?? false) && c.IsActive);
            if (cart == null)
                throw new InvalidOperationException("User cart not found!");
            var paymentGatewayResponse = new PaymentGatewayResponse();
            CustomerAddressDTO customerAddress = null;
            var strEncRequest = string.Empty;
            var responseUrl = string.Empty;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
            var customer = await Customer.GetCustomerByUserId(_context, user.Id);
            if (model.DifferentBillingAddress)
                customerAddress = await CustomerAddressDTO.SaveAddressAsync(_context, customer.Id, model.CustomerAddress ?? new CustomerAddressDTO());
            else
                customerAddress = await CustomerAddressDTO.GetAddress(_context, model.Booking.BillingAddressId ?? 0);
            using var transactionScope = _context.Database.BeginTransaction();
            var bookingDetail = await SetBooking(customer.Id, cart, (int)model.Booking.PaymentStatus);
            bookingDetail.BillingAddressId = customerAddress.Id;
            await _context.Bookings.AddAsync(bookingDetail);
            await _context.SaveChangesAsync();
            var transaction = SetTransaction(bookingDetail);
            transaction.PaymentProvider = (int)model.Booking.PaymentProvider;
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
            if (model.Booking.WalletType != WalletType.None)
            {
                var customerWallet = await _context.Wallets.FirstOrDefaultAsync(s => s.CustomerId == bookingDetail.CustomerId && s.WalletType == (int)model.Booking.WalletType);
                if (customerWallet != null)
                {
                    var debitAmount = (bookingDetail.Total > (customerWallet.BalanceAmount ?? 0)) ? Math.Round(transaction.Amount - customerWallet.BalanceAmount ?? 0, 2) : bookingDetail.Total;
                    transaction.Amount = debitAmount <= transaction.Amount ? bookingDetail.PaymentStatus == (int)PaymentStatus.Advance ? Math.Round((bookingDetail.Total - customerWallet.BalanceAmount ?? 0) / 2, 2) : Math.Round((bookingDetail.Total - customerWallet.BalanceAmount ?? 0), 2) : 0;
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
                    paymentGatewayResponse = await initiateCashFreePaymentRequestAsync(bookingDetail, transaction, user);
                    responseUrl = Url.Action("CashFreePayment", "Booking", new { paymentId = paymentGatewayResponse.PaymentSessionId }, Request.Scheme);
                }
                if (model.Booking.PaymentProvider == PaymentProvider.CCAvenue)
                {
                    var order = GetOrderRequest(bookingDetail, transaction, user);
                    var ccaCrypto = new CCACrypto();
                    string ccaRequest = "merchant_id=" + CCAvenueConfig.MerchantId + "&order_id=" + bookingDetail.Id.ToString(Defaults.BookingPrefix) + "&amount=" + 1 + "&currency=" + Defaults.DefaultCurrency + "&redirect_url=" + Url.Action("ThankYou", "Booking", new { TXNID = transaction.Id }, Request.Scheme) + "&cancel_url=" + Url.Action("ThankYou", "Booking", new { TXNID = transaction.Id }, Request.Scheme) + "&billing_name=" + customerAddress.Name +
                    "&customer_identifier=" + bookingDetail.CustomerId.ToString() + "&merchant_param1=" + transaction.Id.ToString(Defaults.TransactionPrefix) +
                    "&billing_address=" + CustomerAddressDTO.GetAddress(customerAddress) + "&billing_city=" + customerAddress.City + "&billing_state=" + customerAddress.State + "&billing_zip=" + customerAddress.PinCode + "&billing_country=India&billing_email=" + user.Email + "&billing_tel=" + user.PhoneNumber;
                    strEncRequest = ccaCrypto.Encrypt(ccaRequest, CCAvenueConfig.WorkingKey);
                    paymentGatewayResponse = initiateCCAvenuePaymentRequest(bookingDetail, transaction);
                    responseUrl = Url.Action("CCAvenuePayment", "Booking", new { encRequest = strEncRequest }, Request.Scheme);
                }
                await _context.PaymentGatewayResponses.AddAsync(paymentGatewayResponse);
                await _context.SaveChangesAsync();
            }
            else
            {
                bookingDetail.BookingStatus = (int)BookingStatus.Booked;
                transaction.Status = (int)TransactionStatus.Success;
                await _context.SaveChangesAsync();
            }
            await transactionScope.CommitAsync();
            if (bookingDetail.BookingStatus == (int)BookingStatus.Booked)
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
            responseUrl = responseUrl != string.Empty ? responseUrl : Url.Action("ThankYou", "Booking", new { TXNID = transaction.Id, payment_id = transaction.PaymentMode }, Request.Scheme);
            return Redirect(responseUrl);
        }

        private PaymentGatewayResponse initiateCCAvenuePaymentRequest(Booking booking, Transaction transaction)
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

        private async Task<PaymentGatewayResponse> initiateCashFreePaymentRequestAsync(Booking bookingDetail, Transaction transaction, ApplicationUser user)
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

        public async Task<IActionResult> TwoStepBooking()
        {
            var bookingDetail = new Booking();
            try
            {
                var activeCart = Common.GetCookie(Request, "_crt");
                if (activeCart == null)
                    return RedirectToAction("Index", "Service");
                var cart = Converter.ConvertJsonToObject<CartDTO>(Encryption.Decrypt(activeCart));
                var cartInDb = await _context.Carts.FirstOrDefaultAsync(c => c.Id == cart.Id && !(c.IsCheckedOut ?? false) && c.IsActive) ?? throw new InvalidOperationException("User cart not found!");
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
                var customer = await Customer.GetCustomerByUserId(_context, GetUserId());
                bookingDetail = await SetBooking(customer.Id, cartInDb, (int)PaymentStatus.Pending);
                await _context.Bookings.AddAsync(bookingDetail);
                cartInDb.IsCheckedOut = true;
                cartInDb.ModifiedDate = Defaults.GetDateTime();
                cartInDb.ModifiedBy = GetUserId();
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Failed", "Booking", new TransactionDTO { BookingId = bookingDetail.Id });
            }
            return View("ThankYou", new TransactionDTO { BookingId = bookingDetail.Id });
        }

        public IActionResult CashFreePayment(string paymentId)
        {
            return View(new PaymentGatewayResponse { PaymentSessionId = paymentId });
        }

        public IActionResult CCAvenuePayment(string encRequest)
        {
            return View(new CCAvenuePymentRequestDTO { encRequest = encRequest, access_code = CCAvenueConfig.AccessCode });
        }

        public IActionResult Failed(TransactionDTO transaction)
        {
            return View(transaction);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ThankYou(string TXNID, string order_id)
        {
            var transaction = await _context.Transactions.FirstOrDefaultAsync(c => c.Id == Convert.ToInt64(TXNID));
            var bookingStatus = BookingStatus.Failed;
            try
            {
                if (transaction.Status == (int)TransactionStatus.Pending)
                {
                    var booking = await _context.Bookings.FirstOrDefaultAsync(c => c.Id == transaction.BookingId);
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
                                transaction.Status = (Cashfree.OrderStatus)Enum.Parse(typeof(Cashfree.OrderStatus), orderResponse.order_status) == Cashfree.OrderStatus.PAID ? (int)TransactionStatus.Success : (int)TransactionStatus.Failed;
                                paymentGatewayResponse.OrderStatus = (int)Enum.Parse(typeof(Cashfree.OrderStatus), orderResponse.order_status);
                            }
                            else
                            {
                                booking.BookingStatus = (int)BookingStatus.Failed;
                                booking.PaymentStatus = (int)PaymentStatus.Pending;
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
                            }
                            else
                            {
                                booking.BookingStatus = (int)BookingStatus.Failed;
                                booking.PaymentStatus = (int)PaymentStatus.Pending;
                                transaction.Status = (int)TransactionStatus.Failed;
                            }
                        }
                        transaction.ModifiedBy = transaction.CreatedBy;
                        transaction.ModifiedDate = Defaults.GetDateTime();
                        booking.ModifiedBy = transaction.ModifiedBy;
                        booking.ModifiedDate = Defaults.GetDateTime();
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        booking.BookingStatus = (int)BookingStatus.WaitingForApproval;
                        await _context.SaveChangesAsync();
                    }
                    if (booking.BookingStatus == (int)BookingStatus.Booked)
                    {
                        try
                        {
                            var customer = await _context.Customers.Include(c => c.User).FirstOrDefaultAsync(u => u.Id == booking.CustomerId);
                            var serviceDetail = await ServicePriceDTO.GetServicePrice(_context, booking.ServicePriceId);
                            var calenderEvent = new EventDTO
                            {
                                Date = DateOnly.FromDateTime(booking.BookingDate),
                                EndDate = DateOnly.FromDateTime(booking.BookingEndDate),
                                StartTime = TimeOnly.FromDateTime(Convert.ToDateTime(booking.StartTime)),
                                EndTime = TimeOnly.FromDateTime(Convert.ToDateTime(booking.EndTime)),
                                Title = customer.Name,
                                Description = booking.Id.ToString(Defaults.BookingPrefix) + " - " + serviceDetail.ServiceName + " - " + serviceDetail.CategoryName,
                                Email = customer.User.Email,
                                CalendarName = serviceDetail.CalenderName,
                                Studio = serviceDetail.CategoryName,
                                ColorId = serviceDetail.EventColorId ?? "1"
                            };
                            booking.CalenderEventId = await _googleCalendar.AddCalenderEventAsync(calenderEvent);
                            await _context.SaveChangesAsync();

                            //ConfirmBooking
                            var websiteSetting = await WebsiteSettingDTO.GetWebsiteSettingAsync(_context);
                            var bookingDteail = await BookingDTO.GetBookingDetailById(_context, booking.Id);
                            bookingDteail.ServiceName = serviceDetail.ServiceName;
                            bookingDteail.CategoryName = serviceDetail.CategoryName;
                            bookingDteail.ServiceTitle = Defaults.GetServiceTitle(serviceDetail.ServiceName);                            
                            await _emailNotification.SendBookingNotification(bookingDteail, customer.User, websiteSetting);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Error at adding calender event: " + ex.Message, ex);
                        }
                    }
                    bookingStatus = (BookingStatus)booking.BookingStatus;
                }
                if (transaction.Status == (int)TransactionStatus.Failed)
                {
                    if (transaction.PaymentMode == (int)Enums.PaymentMode.Wallet || transaction.PaymentMode == (int)(Enums.PaymentMode.Wallet | Enums.PaymentMode.Online))
                    {
                        var walletTransaction = await _context.WalletTransactions.Include(s => s.Wallet).FirstOrDefaultAsync(s => s.Description == transaction.Id.ToString(Defaults.TransactionPrefix));
                        if (walletTransaction != null)
                        {
                            var refundWalletPayment = new WalletTransactionDTO { Amount = walletTransaction.Amount, Description = transaction.Id.ToString(Defaults.TransactionPrefix), TransactionType = TransactionType.Credit, ReferenceId = transaction.BookingId?.ToString(Defaults.BookingPrefix), Mode = (WalletType)walletTransaction.Wallet.Mode };
                            await WalletDTO.AddUpdateWallet(_logger, _context, walletTransaction.Wallet, refundWalletPayment, transaction.CreatedBy);
                        }
                    }
                    throw new InvalidOperationException("Payment failed by payment gateway.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at Thankyou: " + ex.Message);
                return RedirectToAction("Failed", "Booking", new TransactionDTO { Id = transaction.Id, BookingId = transaction.BookingId, BookingStatus = bookingStatus });
            }
            return View(new TransactionDTO { Id = transaction.Id, BookingId = transaction.BookingId, BookingStatus = bookingStatus });
        }

        private async Task<Booking> SetBooking(long customerId, Cart cart, int paymentType)
        {

            var servicePrice = await ServicePriceDTO.GetServicePrice(_context, cart.ServicePriceId);
            var endTime = DateTime.Parse(cart.EndTime);
            var startTime = DateTime.Parse(cart.StartTime);
            var totalHours = (endTime - startTime).TotalHours;
            var booking = new Booking
            {
                BookingDate = DateTime.Parse(cart.BookingDate),// DateTime.Parse(cart.BookingDate),
                BookingEndDate = DateTime.Parse(cart.BookingEndDate),
                ServicePriceId = servicePrice.Id,
                RatePerHour = (decimal)servicePrice.Price,
                StartTime = cart.StartTime,
                EndTime = cart.EndTime,
                Total = (totalHours * servicePrice.Price),
                IsActive = true,
                BookingStatus = servicePrice.EnableTwoStepBooking ? (int)BookingStatus.WaitingForApproval : (int)BookingStatus.Pending,
                PaymentStatus = paymentType == (int)PaymentType.Advance ? (int)PaymentStatus.Advance : paymentType == (int)PaymentType.Full ? (int)PaymentStatus.FullPayment : (int)PaymentStatus.Pending,
                CustomerId = customerId,
                CreatedBy = GetUserId(),
                CreatedDate = Defaults.GetDateTime(),
                ModifiedBy = GetUserId(),
                ModifiedDate = Defaults.GetDateTime()
            };
            return booking;
        }

        private Transaction SetTransaction(Booking booking)
        {
            return new Transaction
            {
                BookingId = booking.Id,
                PaymentType = booking.PaymentStatus,
                Status = (int)TransactionStatus.Pending,
                TransactionType = (int)TransactionType.Debit,
                PaymentMode = (int)Enums.PaymentMode.Wallet,
                Amount = booking.PaymentStatus == (int)PaymentStatus.Advance ? booking.Total / 2 : booking.Total,
                CustomerId = booking.CustomerId,
                IsActive = true,
                CreatedBy = GetUserId(),
                CreatedDate = Defaults.GetDateTime(),
                ModifiedBy = GetUserId(),
                ModifiedDate = Defaults.GetDateTime()
            };
        }
    }
}
