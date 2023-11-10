using System.ComponentModel.DataAnnotations.Schema;

namespace StudioBooking.Data.Models
{
    public class Wallet : BaseEntity
    {
        public long Id { get; set; }
        [ForeignKey("Customer")]
        public long? CustomerId { get; set; }
        public int WalletType { get; set; }
        public int Mode { get; set; }
        public double? BalanceAmount { get; set; }
        public double? BalanceHours { get; set; }
        public Customer? Customer { get; set; }
        public ICollection<WalletTransaction> WalletTransactions { get; set; }
    }
}