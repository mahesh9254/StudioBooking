using StudioBooking.DTO;

namespace StudioBooking.ViewModels
{
    public class LayoutViewModel
    {
        public string? UserName { get; internal set; }
        public string? ProfilePic { get; internal set; }
        public string? FirstName { get; internal set; }
        public string? LastName { get; internal set; }
        public string? Role { get; internal set; }
        public string? Mobile { get; internal set; }
        public WebsiteSettingDTO WebsiteSetting { get; set; }
        public IEnumerable<CategoryDTO> Categories { get; set; }
        public LayoutViewModel()
        {
            Categories = new List<CategoryDTO>();
        }
    }
}
