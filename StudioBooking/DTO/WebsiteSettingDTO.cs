using Microsoft.EntityFrameworkCore;
using StudioBooking.Data;
using System.ComponentModel.DataAnnotations;

namespace StudioBooking.DTO
{
    public class WebsiteSettingDTO
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string? Logo { get; set; }
        public IFormFile? LogoName { get; set; }
        [MaxLength(100)]
        public string? FooterLogo { get; set; }
        public IFormFile? FooterLogoName { get; set; }
        [MaxLength(100)]
        public string? Favicon { get; set; }
        public IFormFile? FaviconName { get; set; }
        [MaxLength(50)]
        public string? TagLine { get; set; }
        [MaxLength(20)]
        public string? Mobile { get; set; }
        [MaxLength(50)]
        public string? Email { get; set; }
        public string? Aboutus { get; set; }
        public string? TermsAndConditions { get; set; }
        public string? Address { get; set; }
        public string? GoogleMapUrl { get; set; }
        public string? Name { get; set; }
        public int? Year { get; set; }
        public string? CopyRightText { get; set; }
        public string? FacebookLink { get; set; }
        public string? TwitterLink { get; set; }
        public string? LinkedinLink { get; set; }
        public string? InstagramLink { get; set; }
        public string? DiscordLink { get; set; }
        public string? EmailAccount { get; set; }
        public string? EmailAccountUserId { get; set; }
        public string? EmailAccountPassword { get; set; }
        public string? EmailAccountSmtp { get; set; }

        public static async Task<WebsiteSettingDTO> GetWebsiteSettingAsync(ApplicationDbContext context)
        {
            var websiteSetting = await context.WebsiteSettings.FirstOrDefaultAsync();
            if (websiteSetting == null)
                return new WebsiteSettingDTO();

            return new WebsiteSettingDTO
            {
                Name = websiteSetting.Name,
                Email = websiteSetting.Email,
                Mobile = websiteSetting.Mobile,
                TagLine = websiteSetting.TagLine,
                Address = websiteSetting.Address,
                Logo = websiteSetting.Logo,
                FooterLogo = websiteSetting.FooterLogo,
                Favicon = websiteSetting.Favicon,
                Aboutus = websiteSetting.Aboutus,
                TermsAndConditions = websiteSetting.TermsAndConditions,
                CopyRightText = websiteSetting.CopyRightText,
                DiscordLink = websiteSetting.DiscordLink,
                FacebookLink = websiteSetting.FacebookLink,
                InstagramLink = websiteSetting.InstagramLink,
                TwitterLink = websiteSetting.TwitterLink,
                GoogleMapUrl = websiteSetting.GoogleMapUrl,
                Year = websiteSetting.Year,
                LinkedinLink= websiteSetting.LinkedinLink,
                EmailAccount= websiteSetting.EmailAccount,
                EmailAccountPassword= websiteSetting.EmailAccountPassword,
                EmailAccountUserId= websiteSetting.EmailAccountUserId,
                EmailAccountSmtp= websiteSetting.EmailAccountSmtp
            };
        }
    }
}
