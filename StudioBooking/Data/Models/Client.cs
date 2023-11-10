using System.ComponentModel.DataAnnotations;

namespace StudioBooking.Data.Models
{
    public class Client : BaseEntity
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string? Name { get; set; }
        [MaxLength(500)]
        public string? Summary { get; set; }
        [MaxLength(100)]
        public string? Image { get; set; }
        public bool EnablePulse { get; set; }
        public int? Order { get; set; }
    }
}
