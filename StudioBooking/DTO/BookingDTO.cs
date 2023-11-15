using Microsoft.EntityFrameworkCore;
using StudioBooking.Data;
using StudioBooking.Data.Models;
using StudioBooking.Infrastructure;
using static StudioBooking.Infrastructure.Enums;

namespace StudioBooking.DTO
{
    public class BookingDTO
    {
        public long Id { get; set; }
        public string? BookingId { get; set; }
        public int ServicePriceId { get; set; }
        public string? ServiceName { get; set; }
        public long? CustomerId { get; set; }

        public string? BookingDate { get; set; }
        public string? BookingEndDate { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public decimal RatePerHour { get; set; }
        public double Total { get; set; }
        public double AdvancePaid { get; set; }
        public bool IsAddonRequested { get; set; }
        public bool IsBookingExpired { get; set; }
        public long? BillingAddressId { get; set; }
        public string? CategoryName { get; set; }
        public string? ServiceTitle { get; set; }
        public PaymentMode PaymentMode { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public PaymentProvider PaymentProvider { get; set; }
        public WalletType WalletType { get; set; }
        public double TotalHours
        {
            get
            {
                if (string.IsNullOrEmpty(StartTime) || string.IsNullOrEmpty(EndTime))
                    return 0;
                var endTime = TimeOnly.Parse(EndTime);
                var startTime = TimeOnly.Parse(StartTime);
                var startDate = DateOnly.Parse(BookingDate);
                var endDate = DateOnly.Parse(BookingEndDate);
                return (endDate.ToDateTime(endTime) - startDate.ToDateTime(startTime)).TotalHours;                
            }
        }
        public double RefundAmount { get; set; }
        public string? Notes { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedDate { get; set; }


        public static async Task<List<BookingDTO>> GetUserBookings(ApplicationDbContext context, string userId, DateTime date)
        {
            return await context.Bookings.Include(b => b.Customer).Include(b => b.ScheduleRequests).Include(b => b.ServicePrice.Service).Where(b => b.Customer.UserId == userId && !b.IsDelete && b.IsActive && b.BookingDate >= date.Date && ((b.BookingStatus == (int)BookingStatus.Booked) || (b.BookingStatus == (int)BookingStatus.ReScheduled))).Select(b => new BookingDTO
            {
                Id = b.Id,
                BookingId = b.Id.ToString(Defaults.BookingPrefix),
                CustomerId = b.CustomerId,
                BookingDate = b.BookingDate.ToString(Defaults.DefaultDateFormat),
                BookingEndDate = b.BookingEndDate.ToString(Defaults.DefaultDateFormat),
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                RatePerHour = b.RatePerHour,
                Total = b.Total,
                IsBookingExpired = b.BookingDate < Defaults.GetDateTime(),
                IsAddonRequested = b.IsAddonRequested,
                BillingAddressId = b.BillingAddressId,
                ServiceName = b.ServicePrice.Service.Name,
                BookingStatus = b.ScheduleRequests.Any(b => b.RequestType == (int)RequestType.Cancel && b.IsActive && !b.IsDelete && b.RequestStatus == (int)RequestStatus.Pending) ? BookingStatus.WaitingForCancellation : (BookingStatus)b.BookingStatus,
                PaymentStatus = b.ScheduleRequests.Any(b => b.RequestType == (int)RequestType.Cancel && b.IsActive && !b.IsDelete && b.RequestStatus == (int)RequestStatus.Pending) ? PaymentStatus.RefundPending : (PaymentStatus)b.PaymentStatus,
                Notes = b.Notes
            }).ToListAsync();
        }

        public static async Task<List<BookingDTO>> GetBookingsByCategoryId(ApplicationDbContext context, long categoryId, DateTime? date)
        {
            return await context.Bookings.Include(b => b.ServicePrice).Where(b => b.BookingDate == date && !b.IsDelete && b.IsActive && b.ServicePrice.CategoryId == categoryId).Select(s => GetBooking(s)).ToListAsync();
        }
        public static async Task<List<BookingDTO>> GetEndBookingsByCategoryId(ApplicationDbContext context, long categoryId, DateTime? date)
        {
            return await context.Bookings.Include(b => b.ServicePrice).Where(b => b.BookingEndDate == date && !b.IsDelete && b.IsActive && b.ServicePrice.CategoryId == categoryId).Select(s => GetBooking(s)).ToListAsync();
        }

        public static async Task<BookingDTO> GetBookingById(ApplicationDbContext context, long id)
        {
            var booking = await context.Bookings.FirstOrDefaultAsync(b => b.Id == id && !b.IsDelete && b.IsActive);
            return GetBooking(booking ?? new Booking());
        }

        public static async Task<BookingDTO> GetBookingDetailById(ApplicationDbContext context, long id)
        {
            var booking = await context.Bookings.Include(b => b.ScheduleRequests).Include(b => b.Transactions).FirstOrDefaultAsync(b => b.Id == id && !b.IsDelete && b.IsActive);
            return GetBooking(booking ?? new Booking());
        }

        public static BookingDTO GetBooking(Booking booking)
        {
            var isCancelRequested = booking.ScheduleRequests.Any(b => b.RequestType == (int)RequestType.Cancel && b.IsActive && !b.IsDelete && b.RequestStatus == (int)RequestStatus.Pending);
            var advancePaid = booking.Transactions.Count == 0 ? 0 : booking.Total - booking.Transactions.Where(t => t.Status == (int)TransactionStatus.Success).Sum(b => b.Amount);
            return new BookingDTO
            {
                Id = booking.Id,
                BookingId = booking.Id.ToString(Defaults.BookingPrefix),
                CustomerId = booking.CustomerId,
                ServicePriceId = booking.ServicePriceId,
                BookingDate = booking.BookingDate.ToString(Defaults.DefaultDateFormat),
                BookingEndDate = booking.BookingEndDate.ToString(Defaults.DefaultDateFormat),
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                RatePerHour = booking.RatePerHour,
                Total = booking.Total,
                AdvancePaid = booking.Total < advancePaid ? 0 : advancePaid,
                IsBookingExpired = booking.BookingDate < Defaults.GetDateTime(),
                IsAddonRequested = booking.IsAddonRequested,
                BillingAddressId = booking.BillingAddressId,
                BookingStatus = isCancelRequested ? BookingStatus.WaitingForCancellation : (BookingStatus)booking.BookingStatus,
                PaymentStatus = isCancelRequested ? PaymentStatus.RefundPending : (PaymentStatus)booking.PaymentStatus,
                Notes = booking.Notes
            };
        }
    }
}
