using StudioBooking.Data;
using StudioBooking.DTO;

namespace StudioBooking.Areas.Admin.Models
{
    public class CategoryViewModel
    {
        public CategoryDTO Category { get; set; }
        public bool Result { get; set; }
        public string? ErrorMessage { get; set; }
        public string? FilePath { get; set; }
        public CategoryViewModel()
        {
            Category = new CategoryDTO();
        }        
    }
}
