using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudioBooking.Data.Models
{
    public class PaymentGatewayResponse : BaseEntity
    {
        public long Id { get; set; }
        [ForeignKey("Transaction")]
        public long? TransactionId { get; set; }
        [ForeignKey("Booking")]
        public long? BookingId { get; set; }
        [MaxLength(255)]
        public string? OrderId { get; set; }
        [MaxLength(255)]
        public string? PaymentSessionId { get; set; }
        [MaxLength(50)]
        public string? OrderAmount { get; set; }
        [MaxLength(50)]
        public string? OrderCurrency { get; set; }
        public DateTime? OrderExpiryTime { get; set; }
        [MaxLength(255)]
        public string? OrderNote { get; set; }
        public int? OrderStatus { get; set; }
        [MaxLength(255)]
        public string? PaymentUrl { get; set; }
        [MaxLength(255)]
        public string? RefundUrl { get; set; }
        [MaxLength(255)]
        public string? SettlementUrl { get; set; }
        [MaxLength(100)]
        public string? BankRefNo { get; set; }
        [MaxLength(100)]
        public string? FailureMessage { get; set; }
        [MaxLength(50)]
        public string? PaymentMode { get; set; }
        [MaxLength(50)]
        public string? CardName { get; set; }
        public int? StatusCode { get; set; }
        [MaxLength(255)]
        public string? StatusMessage { get; set; }
        [MaxLength(50)]
        public string? discount_value { get; set; }
        [MaxLength(50)]
        public string? mer_amount { get; set; }
        [MaxLength(50)]
        public string? eci_value { get; set; }
        [MaxLength(50)]
        public string? response_code { get; set; }
        [MaxLength(50)]
        public string? trans_date { get; set; }
        public Transaction? Transaction { get; set; }
        public Booking? Booking { get; set; }
    }
}
