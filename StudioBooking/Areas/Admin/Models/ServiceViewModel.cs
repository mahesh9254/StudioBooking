using StudioBooking.DTO;

namespace StudioBooking.Areas.Admin.Models
{
    public class ServiceViewModel
    {
        public ServiceDTO Service { get; set; }
        public bool Result { get; set; }
        public string? ErrorMessage { get; set; }
        public ServiceViewModel()
        {
            Service = new ServiceDTO();
        }
    }
}
