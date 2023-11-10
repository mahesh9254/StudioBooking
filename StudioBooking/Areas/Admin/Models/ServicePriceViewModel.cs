using Microsoft.AspNetCore.Mvc.Rendering;
using StudioBooking.DTO;

namespace StudioBooking.Areas.Admin.Models
{
    public class ServicePriceViewModel
    {
        public ServicePriceDTO ServicePrice { get; set; }
        public SelectList? Categories { get; set; }
        public SelectList? Services { get; set; }
        public SelectList? EventColors { get; set; }
        public bool Result { get; set; }
        public string? ErrorMessage { get; set; }
        public ServicePriceViewModel()
        {
            ServicePrice = new ServicePriceDTO();
        }
    }
}
