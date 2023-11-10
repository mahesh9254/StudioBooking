using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudioBooking.Data.Models
{
    public class PaymentReceipt : BaseEntity
    {
        public long Id { get; set; }
        [ForeignKey("Transaction")]
        public long? TransactionId { get; set; }
        public int PaymentProvider { get; set; }
        [MaxLength(255)]
        public string ReferenceId { get; set; }
        public double Amount { get; set; }
        [MaxLength(255)]
        public string Remarks { get; set; }
        public DateTime PaymentDate { get; set; }
        public Transaction? Transaction { get; set; }
    }
}
