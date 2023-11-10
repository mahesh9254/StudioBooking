using StudioBooking.DTO;

namespace StudioBooking.Areas.Admin.Models
{
    public class WebsiteSettingViewModel
    {
        public WebsiteSettingDTO WebsiteSetting { get; set; }
        public string ErrorMessage { get; set; }
        public bool Result { get; set; }
    }
}
