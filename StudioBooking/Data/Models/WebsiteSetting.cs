using System.ComponentModel.DataAnnotations;

namespace StudioBooking.Data.Models
{
    public class WebsiteSetting : BaseEntity
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string? Logo { get; set; }
        [MaxLength(100)]
        public string? FooterLogo { get; set; }
        [MaxLength(100)]
        public string? Favicon { get; set; }
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

    }
}
