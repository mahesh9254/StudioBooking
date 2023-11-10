using Microsoft.AspNetCore.Mvc.Rendering;
using StudioBooking.DTO;

namespace StudioBooking.Areas.Admin.Models
{
    public class CreateBookingViewModel
    {
        public BookingDTO? Booking { get; set; }
        public SelectList? Categories { get; set; }
        public SelectList? ServicePrices { get; set; }
        public SelectList? Customers { get; set; }
        public string? ServiceStartTime { get; set; }
        public string? ServiceEndTime { get; set; }
        public bool Result { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
