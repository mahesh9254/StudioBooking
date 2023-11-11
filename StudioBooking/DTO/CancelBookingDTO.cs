using static StudioBooking.Infrastructure.Enums;

namespace StudioBooking.DTO
{
    public class CancelBookingDTO
    {
        public long Id { get; set; }
        public long? RequestId { get; set; }
        public string? BookingId { get; set; }
        public int ServicePriceId { get; set; }
        public string? ServiceName { get; set; }
        public long? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CategoryName { get; set; }
        public string? BookingDate { get; set; }
        public string? BookingEndDate { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public decimal RatePerHour { get; set; }
        public double Total { get; set; }
        public double RefundAmount { get; set; }        
        public PaymentProvider? PaymentProvider { get; set; }
        public string? Name { get; set; }
        public string? Account { get; set; }
        public string? IFSC { get; set; }
        public PaymentMode PaymentMode { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public PaymentStatus PaymentStatus { get; set; }       
        public WalletType WalletType { get; set; }
        public string? Notes { get; set; }
        public bool IsReadOnly { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedDate { get; set; }
    }
}
