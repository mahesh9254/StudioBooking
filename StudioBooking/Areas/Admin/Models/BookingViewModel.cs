using Microsoft.EntityFrameworkCore;
using StudioBooking.Data;
using StudioBooking.Data.Models;
using StudioBooking.DTO;
using StudioBooking.Infrastructure;
using static StudioBooking.Infrastructure.Enums;

namespace StudioBooking.Areas.Admin.Models
{
    public class BookingViewModel
    {
        public BookingDTO? Booking { get; set; }
        public ServicePriceDTO ServicePrice { get; set; }
        public CustomerDTO? Customer { get; set; }

        public static Task<List<BookingViewModel>> GetBookings(ApplicationDbContext context)
        {
            return context.Bookings.Include(c => c.Customer).Include(s => s.ServicePrice).Include(s => s.ServicePrice.Category).Include(s => s.ServicePrice.Service).Where(b => b.IsActive && !b.IsDelete && b.BookingStatus != (int)BookingStatus.Failed).Select(b => new BookingViewModel
            {
                Booking = BookingDTO.GetBooking(b),
                Customer = CustomerDTO.GetCustomer(b.Customer ?? new Customer()),
                ServicePrice = ServicePriceDTO.GetServicePrice(b.ServicePrice ?? new ServicePrice())
            }).ToListAsync();
        }
    }
}
