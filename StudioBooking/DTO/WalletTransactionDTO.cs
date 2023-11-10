using static StudioBooking.Infrastructure.Enums;

namespace StudioBooking.DTO
{
    public class WalletTransactionDTO
    {
        public long Id { get; set; }        
        public long? WalletId { get; set; }
        public WalletType Mode { get; set; }
        public double? Amount { get; set; }
        public double? Hours { get; set; }
        public TransactionType TransactionType { get; set; }
        public string? ReferenceId { get; set; }        
        public string? Description { get; set; }
    }
}
