using static StudioBooking.Infrastructure.Enums;

namespace StudioBooking.DTO
{
    public class ScheduleRequestDTO
    {
        public long Id { get; set; }
        public long? BookingId { get; set; }
        public RequestType RequestType { get; set; }
        public DateTime? BookingDate { get; set; }
        public DateTime? BookingEndDate { get; set; }
        public DateTime? RequestDate { get; set; }
        public DateTime? RequestEndDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? RequestStartTime { get; set; }
        public DateTime? RequestEndTime { get; set; }
        public RequestStatus RequestStatus { get; set; }
        public PaymentProvider? PaymentProvider { get; set; }
        public bool RedirectToPayment { get; set; }
        public string? Name { get; set; }
        public string? Account { get; set; }
        public string? IFSC { get; set; }
        public string? Description { get; set; }
    }
}
