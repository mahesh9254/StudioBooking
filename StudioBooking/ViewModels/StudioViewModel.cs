using StudioBooking.DTO;

namespace StudioBooking.ViewModels
{
    public class StudioViewModel
    {
        public CategoryDTO Category { get; set; }
        public IEnumerable<CategoryDTO> Categories { get; set; }
        public IEnumerable<CategoryDetailDTO> CategoryDetails { get; set; }
        public IEnumerable<ServiceDTO> Services { get; set; }
        public IEnumerable<ServicePriceDTO> ServicePrices { get; set; }
        public IEnumerable<ClientDTO> Clients { get; set; }
        public IEnumerable<TestimonialDTO> Testimonials { get; set; }
        public IEnumerable<TeamDTO> Teams { get; set; }
        //public IEnumerable<ServiceGalleryDTO> ServiceGalleries { get; set; }
        public StudioViewModel()
        {
            Category = new CategoryDTO();
            Categories = new List<CategoryDTO>();
            CategoryDetails = new List<CategoryDetailDTO>();
            Services = new List<ServiceDTO>();
            ServicePrices = new List<ServicePriceDTO>();
            Clients = new List<ClientDTO>();
            Teams = new List<TeamDTO>();
            //ServiceGalleries = new List<ServiceGalleryDTO>();
        }
    }
}
