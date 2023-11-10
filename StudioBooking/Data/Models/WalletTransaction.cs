using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudioBooking.Data.Models
{
    public class WalletTransaction : BaseEntity
    {
        public long Id { get; set; }
        [ForeignKey("Wallet")]
        public long? WalletId { get; set; }
        public int Mode { get; set; }
        public double? Amount { get; set; }
        public double? Hours { get; set; }
        public int TransactionType { get; set; }
        public Wallet? Wallet { get; set; }
        [MaxLength(255)]
        public string? ReferenceId { get; set; }
        [MaxLength(255)]
        public string? Description { get; set; }
    }
}
