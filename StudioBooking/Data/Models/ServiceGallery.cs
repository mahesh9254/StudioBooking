using System.ComponentModel.DataAnnotations;

namespace StudioBooking.Data.Models
{
    public class ServiceGallery : BaseEntity
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        [MaxLength(250)]
        public string Title { get; set; }
        [MaxLength(250)]
        public string? SubTitle { get; set; }
        [MaxLength(250)]
        public string? Link { get; set; }
        [MaxLength(500)]
        public string Image { get; set; }
    }
}
