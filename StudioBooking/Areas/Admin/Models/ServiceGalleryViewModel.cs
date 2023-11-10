using StudioBooking.DTO;

namespace StudioBooking.Areas.Admin.Models
{
    public class ServiceGalleryViewModel
    {
        public ServiceGalleryDTO ServiceGallery { get; set; }
        public bool Result { get; set; }
        public string? ErrorMessage { get; set; }
        public ServiceGalleryViewModel()
        {
            ServiceGallery = new ServiceGalleryDTO();
        }
    }
}
