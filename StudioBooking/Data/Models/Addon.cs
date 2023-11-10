using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudioBooking.Data.Models
{
    public class Addon : BaseEntity
    {
        public long Id { get; set; }
        [ForeignKey("Booking")]
        public long? BookingId { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        public int AdjustmentType { get; set; }
        public double? Amount { get; set; }
        [MaxLength(500)]
        public string Description { get; set; }
        public Booking Booking { get; set; }
    }
}
