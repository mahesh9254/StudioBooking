using Microsoft.EntityFrameworkCore;
using StudioBooking.Data;
using StudioBooking.Data.Models;
using StudioBooking.Infrastructure;

namespace StudioBooking.DTO
{
    public class EventDTO
    {
        public string Title { get; set; }
        public string Studio { get; set; }
        public string CalendarName { get; set; }
        public DateOnly Date { get; set; }
        public DateOnly EndDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string? Email { get; set; }
        public string Description { get; set; }
        public string ColorId { get; set; }

        public static async Task AddCalenderEvent(ApplicationDbContext context, IGoogleCalendar googleCalendar, Booking booking)
        {
            var customer = await context.Customers.Include(c => c.User).FirstOrDefaultAsync(u => u.Id == booking.CustomerId);
            var serviceDetail = await ServicePriceDTO.GetServicePrice(context, booking.ServicePriceId);
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
            booking.CalenderEventId = await googleCalendar.AddCalenderEventAsync(calenderEvent);
        }

        public static async Task UpdateCalenderEvent(ApplicationDbContext context, IGoogleCalendar googleCalendar, Booking booking)
        {
            var customer = await context.Customers.Include(c => c.User).FirstOrDefaultAsync(u => u.Id == booking.CustomerId);
            var serviceDetail = await ServicePriceDTO.GetServicePrice(context, booking.ServicePriceId);
            var calenderEvent = new EventDTO
            {
                Date = DateOnly.FromDateTime(booking.BookingDate),
                StartTime = TimeOnly.FromDateTime(Convert.ToDateTime(booking.StartTime)),
                EndTime = TimeOnly.FromDateTime(Convert.ToDateTime(booking.EndTime)),
                EndDate = DateOnly.FromDateTime(booking.BookingEndDate),
                Title = customer.Name,
                Description = booking.Id.ToString(Defaults.BookingPrefix) + " - " + serviceDetail.ServiceName + " - " + serviceDetail.CategoryName,
                Email = customer.User.Email,
                CalendarName = serviceDetail.CalenderName,
                Studio = serviceDetail.CategoryName,
                ColorId = serviceDetail.EventColorId ?? "1"
            };
            await googleCalendar.UpdateCalenderEvents(booking.CalenderEventId, calenderEvent);
        }
    }
}
