using Cashfree;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudioBooking.Areas.Admin.Models;
using StudioBooking.Data;
using StudioBooking.Data.Models;
using StudioBooking.DTO;
using StudioBooking.Infrastructure;
using static StudioBooking.Infrastructure.Enums;
using System.Net;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace StudioBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BookingController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ICashFreeClient _cashFreeClient;
        private readonly IGoogleCalendar _googleCalendar;
        private readonly IEmailNotification _emailNotification;
        private readonly ILogger<BookingController> _logger;
        public BookingController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ICashFreeClient cashFreeClient, IGoogleCalendar googleCalendar, IEmailNotification emailNotification, ILogger<BookingController> logger) : base(userManager)
        {
            _context = context;
            _cashFreeClient = cashFreeClient;
            _googleCalendar = googleCalendar;
            _emailNotification = emailNotification;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetBookings()//[FromForm] DataTableSearchModel searchModel
        {
            var bookings = await BookingViewModel.GetBookings(_context);
            //return Ok(new { recordsTotal = bookings.Count, recordsFiltered = bookings.Count, data = bookings.OrderByDescending(b => b.Booking.Id).Take(10).ToList() });
            return Ok(new { data = bookings.OrderByDescending(b => b.Booking.Id).ToList() });
        }

        [HttpGet]
        public async Task<IActionResult> CreateBookingModal(long? id)
        {
            if (id == null)
            {
                var model = new CreateBookingViewModel
                {
                    Booking = new BookingDTO(),
                    Customers = new SelectList(await _context.Customers.Where(c => c.IsActive && c.IsDelete != true).Select(s => new { s.Id, s.Name }).ToListAsync(), "Id", "Name", null),
                    Categories = new SelectList(await _context.Categories.Where(c => c.IsActive && c.IsDelete != true).Select(s => new { s.Id, s.Name }).ToListAsync(),
                    "Id",
                    "Name",
                    null),
                    ServicePrices = new SelectList(new List<SelectListItem>(), "Id", "Name", null)
                };
                return PartialView("_BookingModal", model);
            }
            else
            {
                var booking = await BookingDTO.GetBookingById(_context, id ?? 0);
                //booking.BookingDate = DateTime.ParseExact(booking.BookingDate, "dd/MM/yyyy", null).ToString();
                //booking.BookingEndDate = DateTime.ParseExact(booking.BookingEndDate, "dd/MM/yyyy", null).ToString();
                var servicePrice = await _context.ServicePrices.Include(s => s.Category).FirstOrDefaultAsync(s => s.Id == booking.ServicePriceId);
                var model = new CreateBookingViewModel
                {
                    Booking = booking,
                    Customers = new SelectList(await _context.Customers.Where(c => c.IsActive && c.IsDelete != true).Select(s => new { s.Id, s.Name }).ToListAsync(), "Id", "Name", booking.CustomerId),
                    Categories = new SelectList(await _context.Categories.Where(c => c.IsActive && c.IsDelete != true).Select(s => new { s.Id, s.Name }).ToListAsync(),
                   "Id",
                   "Name",
                   servicePrice.CategoryId),
                    ServicePrices = new SelectList(await ServicePriceDTO.GetServicePricesByCategory(_context, servicePrice.CategoryId), "Id", "ServiceName", servicePrice.Id),
                    ServiceStartTime = servicePrice.Category.StartTime,
                    ServiceEndTime = servicePrice.Category.EndTime
                };
                return PartialView("_BookingModal", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> PaymentReceiptModal(long id)
        {
            try
            {
                var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id && b.PaymentStatus != (int)PaymentStatus.FullPayment && (b.BookingStatus == (int)BookingStatus.Pending || b.BookingStatus == (int)BookingStatus.Booked || b.BookingStatus == (int)BookingStatus.ReScheduled)) ?? throw new Exception("Booking is not eligible for payments");

                var model = new PaymentReceiptViewModel
                {
                    PaymentReceipt = new PaymentReceiptDTO { BookingId = id }
                };
                return PartialView("_PaymentReceiptModal", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at Admin=>PaymentReceiptModal :" + ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBookingAddonModal(long id)
        {
            try
            {
                var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id && (b.BookingStatus == (int)BookingStatus.Booked || b.BookingStatus == (int)BookingStatus.ReScheduled)) ?? throw new Exception("Booking is not eligible for addons");

                var model = new AddOnDTO
                {
                    BookingId = id,
                    BookingTotal = booking.Total
                };
                return PartialView("_BookingAddonModal", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at Admin=>GetBookingAddonModal :" + ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBookingAddon(AddOnDTO model)
        {
            try
            {
                var booking = await _context.Bookings.Include(b => b.Transactions.Where(t => t.Status == (int)TransactionStatus.Success)).FirstOrDefaultAsync(b => b.Id == model.BookingId && (b.BookingStatus == (int)BookingStatus.Booked || b.BookingStatus == (int)BookingStatus.ReScheduled)) ?? throw new Exception("Booking is not eligible for addons");

                var totalPaid = booking.Transactions.Sum(t => t.Amount);

                await _context.Addons.AddAsync(new Addon
                {
                    Name = model.Name,
                    Amount = model.Amount,
                    BookingId = model.BookingId,
                    Description = model.Description,
                    AdjustmentType = model.AdjustmentType,
                    IsActive = true,
                    CreatedBy = GetUserId(),
                    CreatedDate = Defaults.GetDateTime(),
                    ModifiedBy = GetUserId(),
                    ModifiedDate = Defaults.GetDateTime(),
                });
                booking.Total = model.AdjustmentType == 1 ? (booking.Total + model.Amount ?? 0) : (booking.Total - model.Amount ?? 0);
                booking.PaymentStatus = totalPaid < booking.Total ? (int)PaymentStatus.Advance : (int)PaymentStatus.FullPayment;
                booking.IsAddonRequested = true;
                booking.ModifiedDate = Defaults.GetDateTime();
                booking.ModifiedBy = GetUserId();
                await _context.SaveChangesAsync();
                return Ok(new { data = model, result = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error at Admin=>GetBookingAddonModal :" + ex.Message);
                return Ok(new { data = model, result = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ScheduleChangeRequest([FromBody] BookingDTO modal)
        {
            try
            {
                var booking = await _context.Bookings.Include(b => b.Transactions).FirstOrDefaultAsync(b => b.Id == modal.Id) ?? throw new Exception("Invalid booking id");
                var endTime = DateTime.Parse(modal.EndTime);
                var startTime = DateTime.Parse(modal.StartTime);

                var request = new ScheduleRequest
                {
                    BookingId = booking.Id,
                    BookingDate = booking.BookingDate,
                    BookingEndDate = booking.BookingEndDate,
                    RequestDate = DateTime.ParseExact(modal.BookingDate, "dd/MM/yyyy", null),//DateTime.Parse(modal.BookingDate),
                    RequestEndDate = DateTime.ParseExact(modal.BookingEndDate, "dd/MM/yyyy", null),//DateTime.Parse(modal.BookingDate),
                    StartTime = booking.StartTime,
                    EndTime = booking.EndTime,
                    RequestStartTime = startTime,
                    RequestEndTime = endTime,
                    RequestStatus = (int)RequestStatus.Completed,
                    RequestType = (int)RequestType.ReSchedule,
                    Description = "Re-Scheduled By Studio",
                    IsActive = true,
                    CreatedBy = GetUserId(),
                    CreatedDate = Defaults.GetDateTime(),
                    ModifiedBy = GetUserId(),
                    ModifiedDate = Defaults.GetDateTime()
                };

                await _context.ScheduleRequests.AddAsync(request);
                await _context.SaveChangesAsync();               

                var newTotalHours = (DateOnly.Parse(modal.BookingEndDate).ToDateTime(TimeOnly.Parse(modal.EndTime)) - DateOnly.Parse(modal.BookingDate).ToDateTime(TimeOnly.Parse(modal.StartTime))).TotalHours;
                var oldTotalHours = (booking.BookingEndDate.Add(TimeSpan.Parse(booking.EndTime)) - booking.BookingDate.Add(TimeSpan.Parse(booking.StartTime))).TotalHours;


                var pendingAmount = booking.Total - booking.Transactions.Where(t => t.Status == (int)TransactionStatus.Success).Sum(b => b.Amount);
                booking.BookingDate = request.RequestDate ?? DateTime.MinValue;
                booking.BookingEndDate = request.RequestEndDate ?? DateTime.MinValue;
                booking.StartTime = TimeOnly.FromDateTime(request.RequestStartTime ?? DateTime.MinValue).ToString();
                booking.EndTime = TimeOnly.FromDateTime(request.RequestEndTime ?? DateTime.MinValue).ToString();
                booking.Total = newTotalHours * (double)booking.RatePerHour;
                booking.BookingStatus = pendingAmount == booking.Total ? booking.BookingStatus : (int)BookingStatus.ReScheduled;
                booking.PaymentStatus = pendingAmount > 0 ? (int)PaymentStatus.Advance : (int)PaymentStatus.FullPayment;
                await _context.SaveChangesAsync();
                try
                {
                    if (string.IsNullOrEmpty(booking.CalenderEventId))
                        await EventDTO.AddCalenderEvent(_context, _googleCalendar, booking);
                    else
                        await EventDTO.UpdateCalenderEvent(_context, _googleCalendar, booking);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error at adding calender event ScheduleChangeRequest: " + ex.Message, ex);
                }
                await _context.SaveChangesAsync();

                //Reschedule/Cancellation
                var serviceDetail = await ServicePriceDTO.GetServicePrice(_context, booking.ServicePriceId);
                var bookingDetail = await BookingDTO.GetBookingDetailById(_context, booking.Id);
                bookingDetail.ServiceName = serviceDetail.ServiceName;
                bookingDetail.CategoryName = serviceDetail.CategoryName;
                bookingDetail.ServiceTitle = Defaults.GetServiceTitle(serviceDetail.ServiceName);
                var customer = await _context.Customers.Include(c => c.User).FirstOrDefaultAsync(u => u.Id == booking.CustomerId);
                var websiteSetting = await WebsiteSettingDTO.GetWebsiteSettingAsync(_context);
                await _emailNotification.SendReScheduleCancellationNotification(request, bookingDetail, customer?.User ?? new ApplicationUser(), websiteSetting);
                modal.Id = request.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError("ScheduleChangeRequest => " + ex.Message, ex);
                return Ok(new { data = modal, result = false, error = ex.Message });
            }
            return Ok(new { data = modal, result = true });
        }

        [HttpGet]
        public async Task<IActionResult> GetCancelModal(int id, int type = 0)
        {
            try
            {
                var booking = await _context.Bookings.Include(b => b.Customer).Include(b => b.ServicePrice.Service).Include(b => b.ServicePrice.Category)
                   .Include(b => b.Transactions.Where(t => t.Status == (int)TransactionStatus.Success)).Include(b => b.ScheduleRequests).FirstOrDefaultAsync(b => b.Id == id && (b.BookingStatus == (int)BookingStatus.Pending || b.BookingStatus == (int)BookingStatus.Booked || b.BookingStatus == (int)BookingStatus.ReScheduled || b.BookingStatus == (int)BookingStatus.WaitingForCancellation || b.BookingStatus == (int)BookingStatus.Cancelled)) ?? throw new Exception("Booking is not eligible for cancellation");

                if (type != 3 && booking.BookingStatus == (int)BookingStatus.Cancelled)
                    throw new Exception("Booking is not eligible for cancellation");

                var totalPaid = booking.Transactions.Where(t => t.Status == (int)TransactionStatus.Success).Sum(t => t.Amount);

                var model = new CancelBookingDTO
                {
                    Id = booking.Id,
                    BookingId = booking.Id.ToString(Defaults.BookingPrefix),
                    BookingStatus = (BookingStatus)booking.BookingStatus,
                    PaymentStatus = (PaymentStatus)booking.PaymentStatus,
                    BookingDate = booking.BookingDate.ToString(Defaults.DefaultDateFormat),
                    BookingEndDate = booking.BookingEndDate.ToString(Defaults.DefaultDateFormat),
                    StartTime = booking.StartTime,
                    EndTime = booking.EndTime,
                    RefundAmount = (booking.Total - totalPaid) == 0 ? booking.Total / 2 : 0,
                    ServiceName = booking.ServicePrice.Service.Name,
                    CategoryName = booking.ServicePrice.Category.Name
                };
                if (type != 0)
                {
                    var scheduleRequest = booking.ScheduleRequests.Where(s => s.RequestStatus == (type == 1 ? (int)RequestStatus.Pending : (int)RequestStatus.Completed)).OrderByDescending(s => s.Id).FirstOrDefault();
                    model.PaymentProvider = (PaymentProvider)(scheduleRequest.PaymentProvider ?? (int)PaymentProvider.Cash);
                    model.Name = scheduleRequest.Name;
                    model.Account = scheduleRequest.Account;
                    model.IFSC = scheduleRequest.IFSC;
                    model.Total = totalPaid;
                    model.IsReadOnly = type != 0;
                }
                return PartialView("_CancelModal", model);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetCancelModal => " + ex.Message, ex);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CancelBooking([FromBody] ScheduleRequestDTO modal)
        {
            try
            {
                var userId = GetUserId();
                var booking = await _context.Bookings.Include(b => b.Transactions).FirstOrDefaultAsync(b => b.Id == modal.BookingId) ?? throw new Exception("Invalid booking id");
                var request = new ScheduleRequest
                {
                    BookingId = booking.Id,
                    BookingDate = booking.BookingDate,
                    StartTime = booking.StartTime,
                    EndTime = booking.EndTime,
                    RequestStatus = (int)RequestStatus.Completed,
                    RequestType = (int)modal.RequestType,
                    Description = modal.Description,
                    IsActive = true,
                    CreatedBy = userId,
                    CreatedDate = Defaults.GetDateTime(),
                    ModifiedBy = userId,
                    ModifiedDate = Defaults.GetDateTime(),
                    PaymentProvider = modal.PaymentProvider != null ? (int)modal.PaymentProvider : null,
                    Account = modal.Account,
                    Name = modal.Name,
                    IFSC = modal.IFSC
                };
                booking.BookingStatus = (int)BookingStatus.Cancelled;
                booking.ModifiedDate = Defaults.GetDateTime();
                booking.ModifiedBy = userId;
                await _context.ScheduleRequests.AddAsync(request);
                await _context.SaveChangesAsync();
                var serviceDetail = await ServicePriceDTO.GetServicePrice(_context, booking.ServicePriceId);
                var customer = await _context.Customers.Include(c => c.User).FirstOrDefaultAsync(u => u.Id == booking.CustomerId);
                if (!string.IsNullOrEmpty(booking.CalenderEventId))
                {
                    try
                    {
                        await _googleCalendar.DeletCalendarEvents(serviceDetail.CalenderName, booking.CalenderEventId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("CancelBooking => Error at deleting calender event: " + ex.Message, ex);
                    }
                }

                //Reschedule/Cancellation
                var bookingDetail = await BookingDTO.GetBookingDetailById(_context, booking.Id);
                bookingDetail.ServiceName = serviceDetail.ServiceName;
                bookingDetail.CategoryName = serviceDetail.CategoryName;
                bookingDetail.ServiceTitle = Defaults.GetServiceTitle(serviceDetail.ServiceName);
                var websiteSetting = await WebsiteSettingDTO.GetWebsiteSettingAsync(_context);
                await _emailNotification.SendReScheduleCancellationNotification(request, bookingDetail, customer?.User ?? new ApplicationUser(), websiteSetting);
                return Ok(new { data = modal, result = true });
            }
            catch (Exception ex)
            {
                _logger.LogError("CancelBooking => " + ex.Message, ex);
                return Ok(new { data = modal, result = false, error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateBookingStatus(long id, int status)
        {
            try
            {
                var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id && b.IsActive && !b.IsDelete) ?? throw new Exception("Invalid booking id");
                booking.BookingStatus = status;
                booking.ModifiedDate = Defaults.GetDateTime();
                booking.ModifiedBy = GetUserId();
                if (status == (int)BookingStatus.OnHold)
                {
                    if (!string.IsNullOrEmpty(booking.CalenderEventId))
                    {
                        var customer = await _context.Customers.Include(c => c.User).FirstOrDefaultAsync(u => u.Id == booking.CustomerId);
                        var serviceDetail = await ServicePriceDTO.GetServicePrice(_context, booking.ServicePriceId);
                        await _googleCalendar.DeletCalendarEvents(serviceDetail.CalenderName, booking.CalenderEventId);
                    }
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Ok(new { result = false, error = ex.Message });
            }
            return Ok(new { result = true });
        }

        [HttpPost]
        public async Task<IActionResult> AddBooking([FromBody] BookingDTO booking)
        {
            try
            {
                var newBooking = await SetBooking(booking);
                await _context.Bookings.AddAsync(newBooking);
                try
                {
                    var customer = await _context.Customers.Include(c => c.User).FirstOrDefaultAsync(u => u.Id == booking.CustomerId);
                    var serviceDetail = await ServicePriceDTO.GetServicePrice(_context, booking.ServicePriceId);
                    var calenderEvent = new EventDTO
                    {
                        Date = DateOnly.FromDateTime(newBooking.BookingDate),
                        EndDate = DateOnly.FromDateTime(newBooking.BookingEndDate),
                        StartTime = TimeOnly.FromDateTime(Convert.ToDateTime(newBooking.StartTime)),
                        EndTime = TimeOnly.FromDateTime(Convert.ToDateTime(newBooking.EndTime)),
                        Title = customer.Name,
                        Description = newBooking.Id.ToString(Defaults.BookingPrefix) + " - " + serviceDetail.ServiceName + " - " + serviceDetail.CategoryName,
                        Email = customer.User.Email,
                        CalendarName = serviceDetail.CalenderName,
                        Studio = serviceDetail.CategoryName,
                        ColorId = serviceDetail.EventColorId ?? "1"
                    };
                    newBooking.CalenderEventId = await _googleCalendar.AddCalenderEventAsync(calenderEvent);

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
                    _logger.LogError("Error at Admin=>AddBooking=> calender event: " + ex.Message, ex);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Ok(new { result = false, error = ex.Message });
            }
            return Ok(new { result = true });
        }

        [HttpPost]
        public async Task<IActionResult> AddPayment([FromBody] PaymentReceiptDTO modal)
        {
            try
            {
                using var transactionScope = _context.Database.BeginTransaction();
                var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == (modal.BookingId ?? 0));
                var transaction = new Transaction
                {
                    BookingId = booking.Id,
                    PaymentType = (int)modal.PaymentStatus,
                    PaymentMode = modal.PaymentProvider == PaymentProvider.Other || modal.PaymentProvider == PaymentProvider.Cash ? (int)PaymentMode.Offline : (int)PaymentMode.Online,
                    Status = (int)TransactionStatus.Success,
                    TransactionType = (int)TransactionType.Debit,
                    Amount = modal.Amount,
                    CustomerId = booking.CustomerId,
                    IsActive = true,
                    CreatedBy = GetUserId(),
                    CreatedDate = Defaults.GetDateTime(),
                    ModifiedBy = GetUserId(),
                    ModifiedDate = Defaults.GetDateTime()
                };
                await _context.Transactions.AddAsync(transaction);
                await _context.SaveChangesAsync();
                await _context.PaymentReceipts.AddAsync(new PaymentReceipt
                {
                    Amount = modal.Amount,
                    TransactionId = transaction.Id,
                    IsActive = true,
                    PaymentDate = modal.PaymentDate,
                    ReferenceId = modal.ReferenceId,
                    Remarks = modal.Remarks,
                    PaymentProvider = (int)modal.PaymentProvider,
                    CreatedBy = GetUserId(),
                    CreatedDate = Defaults.GetDateTime(),
                    ModifiedBy = GetUserId(),
                    ModifiedDate = Defaults.GetDateTime(),
                });
                booking.BookingStatus = booking.BookingStatus != (int)BookingStatus.Booked ? (int)BookingStatus.Booked : booking.BookingStatus;
                booking.PaymentStatus = (int)modal.PaymentStatus;
                booking.ModifiedBy = GetUserId();
                booking.ModifiedDate = Defaults.GetDateTime();
                if (string.IsNullOrEmpty(booking.CalenderEventId))
                {
                    var customer = await _context.Customers.Include(c => c.User).FirstOrDefaultAsync(u => u.Id == booking.CustomerId);
                    var serviceDetail = await ServicePriceDTO.GetServicePrice(_context, booking.ServicePriceId);
                    var calenderEvent = new EventDTO
                    {
                        Date = DateOnly.FromDateTime(booking.BookingDate),
                        EndDate = DateOnly.FromDateTime(booking.BookingEndDate),
                        StartTime = TimeOnly.FromDateTime(Convert.ToDateTime(booking.StartTime)),
                        EndTime = TimeOnly.FromDateTime(Convert.ToDateTime(booking.EndTime)),
                        Title = serviceDetail.ServiceName + " - " + serviceDetail.CategoryName,
                        Description = booking.Id.ToString(Defaults.BookingPrefix),
                        Email = customer.User.Email,
                        Studio = serviceDetail.CategoryName,
                        ColorId = serviceDetail.EventColorId ?? "1"
                    };
                    booking.CalenderEventId = await _googleCalendar.AddCalenderEventAsync(calenderEvent);
                }
                await _context.SaveChangesAsync();
                await transactionScope.CommitAsync();
            }
            catch (Exception ex)
            {
                return Ok(new { result = false, error = ex.Message });
            }
            return Ok(new { result = true });
        }

        [HttpGet]
        public async Task<IActionResult> ApproveBooking(long id)
        {
            try
            {
                var booking = await _context.Bookings.Include(b => b.Customer).FirstOrDefaultAsync(b => b.Id == id) ?? throw new Exception("Booking does not exists");
                var customer = await _context.Users.FirstOrDefaultAsync(u => u.Id == booking.Customer.UserId);
                var paymentGatewayResponse = new PaymentGatewayResponse();
                using var transactionScope = _context.Database.BeginTransaction();
                var transaction = SetTransaction(booking);
                await _context.Transactions.AddAsync(transaction);
                await _context.SaveChangesAsync();
                var order = new OrderRequest
                {
                    order_id = booking.Id.ToString(Defaults.BookingPrefix),
                    order_amount = booking.PaymentStatus == (int)PaymentStatus.Advance ? booking.Total / 2 : booking.Total,
                    order_currency = Defaults.DefaultCurrency,
                    order_expiry_time = Defaults.GetDateTime().AddDays(7),
                    order_meta = new OrderMeta
                    {
                        return_url = Url.Action("ThankYou", "Booking", new { TXNID = transaction.Id }, Request.Scheme) + "&order_id={order_id}"
                    },
                    customer_details = new CustomerDetail
                    {
                        customer_id = booking.CustomerId.ToString(),
                        customer_email = customer.Email,
                        customer_phone = customer.PhoneNumber,
                        customer_name = customer.FirstName + " " + customer.LastName,
                    }
                };
                var result = await _cashFreeClient.PostAsync(order, PaymentGatewayAPIs.Order);
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    var response = await result.Content.ReadAsStringAsync();
                    var orderResponse = Converter.ConvertJsonToObject<OrderResponse>(response);
                    paymentGatewayResponse = new PaymentGatewayResponse
                    {
                        TransactionId = transaction.Id,
                        BookingId = booking.Id,
                        OrderId = orderResponse.cf_order_id.ToString(),
                        OrderAmount = orderResponse.order_amount.ToString(),
                        OrderCurrency = orderResponse.order_currency,
                        OrderNote = orderResponse.order_note,
                        OrderExpiryTime = orderResponse.order_expiry_time,
                        PaymentSessionId = orderResponse.payment_session_id,
                        OrderStatus = (int)Enum.Parse(typeof(OrderStatus), orderResponse.order_status),
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
                await _context.PaymentGatewayResponses.AddAsync(paymentGatewayResponse);
                await _context.SaveChangesAsync();
                await transactionScope.CommitAsync();
                var responseUrl = Url.Action("CashFreePayment", "Booking", new { paymentId = paymentGatewayResponse.PaymentSessionId }, Request.Scheme);
            }
            catch (Exception ex)
            {
                _logger.LogError("ApproveBooking => Invalid Booking Id: " + id, ex);
                return Ok(new { result = false, err = ex.Message });
            }
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ApproveCancellationRequest(long id, int status)
        {
            try
            {
                var booking = await _context.Bookings.Include(b => b.ScheduleRequests).Include(b => b.Customer).FirstOrDefaultAsync(b => b.Id == id) ?? throw new Exception("Booking does not exists");
                foreach (var request in booking.ScheduleRequests.Where(b => b.IsActive && !b.IsDelete && b.RequestStatus == (int)RequestStatus.Pending))
                {
                    request.RequestStatus = status;
                }
                booking.BookingStatus = status == (int)RequestStatus.Rejected ? (int)BookingStatus.Booked : (int)BookingStatus.Cancelled;
                booking.PaymentStatus = status == (int)RequestStatus.Rejected ? booking.PaymentStatus : (int)PaymentStatus.RefundCompleted;
                booking.ModifiedDate = Defaults.GetDateTime();
                booking.ModifiedBy = GetUserId();
                if (status == (int)RequestStatus.Completed)
                {
                    if (!string.IsNullOrEmpty(booking.CalenderEventId))
                    {
                        var customer = await _context.Customers.Include(c => c.User).FirstOrDefaultAsync(u => u.Id == booking.CustomerId);
                        var serviceDetail = await ServicePriceDTO.GetServicePrice(_context, booking.ServicePriceId);
                        await _googleCalendar.DeletCalendarEvents(serviceDetail.CalenderName, booking.CalenderEventId);
                    }
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("ApproveCancellationRequest => Invalid Booking Id: " + id, ex);
                return Ok(new { result = false, err = ex.Message });
            }
            return Ok(new { result = true });
        }

        private Transaction SetTransaction(Booking booking)
        {
            return new Transaction
            {
                BookingId = booking.Id,
                PaymentType = booking.PaymentStatus,
                PaymentMode = (int)PaymentMode.Online,
                Status = (int)TransactionStatus.Pending,
                TransactionType = (int)TransactionType.Debit,
                Amount = booking.PaymentStatus == (int)PaymentStatus.Advance ? booking.Total / 2 : booking.Total,
                CustomerId = booking.CustomerId,
                IsActive = true,
                CreatedBy = GetUserId(),
                CreatedDate = Defaults.GetDateTime(),
                ModifiedBy = GetUserId(),
                ModifiedDate = Defaults.GetDateTime()
            };
        }

        private async Task<Booking> SetBooking(BookingDTO booking)
        {
            var servicePrice = await ServicePriceDTO.GetServicePrice(_context, booking.ServicePriceId);
            var endTime = TimeOnly.Parse(booking.EndTime);
            var startTime = TimeOnly.Parse(booking.StartTime);
            var startDate = DateOnly.Parse(booking.BookingDate);
            var endDate = DateOnly.Parse(booking.BookingEndDate);
            var  totalHours = (endDate.ToDateTime(endTime) - startDate.ToDateTime(startTime)).TotalHours;
           
            var customerAddress = await _context.CustomerAddresses.FirstOrDefaultAsync(a => a.IsDefault && a.CustomerId == booking.CustomerId);
            return new Booking
            {
                BookingDate = DateTime.ParseExact(booking.BookingDate, Defaults.DefaultDateFormat, null),
                BookingEndDate = DateTime.ParseExact(booking.BookingEndDate, Defaults.DefaultDateFormat, null),
                ServicePriceId = servicePrice.Id,
                RatePerHour = (decimal)servicePrice.Price,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                Total = (totalHours * servicePrice.Price),
                IsActive = true,
                BookingStatus = (int)BookingStatus.Reserved,
                PaymentStatus = (int)PaymentStatus.Pending,
                CustomerId = booking.CustomerId,
                BillingAddressId = customerAddress != null ? customerAddress.Id : null,
                CreatedBy = GetUserId(),
                CreatedDate = Defaults.GetDateTime(),
                ModifiedBy = GetUserId(),
                ModifiedDate = Defaults.GetDateTime()
            };
        }
    }
}
