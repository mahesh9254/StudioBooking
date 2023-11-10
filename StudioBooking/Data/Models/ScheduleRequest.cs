using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static StudioBooking.Infrastructure.Enums;

namespace StudioBooking.Data.Models
{
    public class ScheduleRequest : BaseEntity
    {
        public long Id { get; set; }
        [ForeignKey("Booking")]
        public long? BookingId { get; set; }
        public int RequestType { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public DateTime? BookingDate { get; set; }
        public DateTime? RequestDate { get; set; }
        public DateTime? RequestStartTime { get; set; }
        public DateTime? RequestEndTime { get; set; }
        public int RequestStatus { get; set; }
        public string? Description { get; set; }
        public int? PaymentProvider { get; set; }
        [MaxLength(100)]
        public string? Name { get; set; }
        [MaxLength(100)]
        public string? Account { get; set; }
        [MaxLength(50)]
        public string? IFSC { get; set; }
        public Booking? Booking { get; set; }
    }
}
