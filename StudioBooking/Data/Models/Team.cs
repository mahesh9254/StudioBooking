using System.ComponentModel.DataAnnotations;

namespace StudioBooking.Data.Models
{
    public class Team : BaseEntity
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string? Name { get; set; }
        [MaxLength(200)]
        public string? Professions { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        [MaxLength(100)]
        public string? Image { get; set; }
    }
}
