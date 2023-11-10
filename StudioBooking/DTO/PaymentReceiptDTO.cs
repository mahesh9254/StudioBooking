using System.ComponentModel.DataAnnotations;
using static StudioBooking.Infrastructure.Enums;

namespace StudioBooking.DTO
{
    public class PaymentReceiptDTO
    {
        public long Id { get; set; }        
        public long? TransactionId { get; set; }
        public long? BookingId { get; set; }
        public PaymentProvider PaymentProvider { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        [MaxLength(255)]
        public string ReferenceId { get; set; }
        public double Amount { get; set; }
        [MaxLength(255)]
        public string Remarks { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
