using StudioBooking.DTO;

namespace StudioBooking.ViewModels
{
    public class ServiceViewModel
    {
        public CartDTO? Cart { get; set; }
        public ServicePriceDTO? ServicePrice { get; set; }
        public ServiceViewModel()
        {
            Cart = new CartDTO();
            ServicePrice = new ServicePriceDTO();
        }
    }
}
