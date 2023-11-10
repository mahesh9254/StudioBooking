using StudioBooking.DTO;

namespace StudioBooking.Areas.Admin.Models
{
    public class CategoryDetailViewModel
    {
        public CategoryDTO? Category { get; set; }
        public CategoryDetailDTO CategoryLiveRoom { get; set; }
        public CategoryDetailDTO CategoryControlRoom { get; set; }
        public bool Result { get; set; }
        public string? ErrorMessage { get; set; }
        public string? FilePath { get; set; }
        public CategoryDetailViewModel()
        {
            CategoryLiveRoom = new CategoryDetailDTO();
            CategoryControlRoom = new CategoryDetailDTO();
        }
    }
}
