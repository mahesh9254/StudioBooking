using StudioBooking.Data.Models;
using StudioBooking.DTO;

namespace StudioBooking.ViewModels
{
    public class TransactionViewModel
    {
        public TransactionDTO Transaction { get; set; }
        public WalletTransactionDTO WalletTransaction { get; set; }
        public PaymentGatewayResponse PaymentGatewayResponse { get; set; }
        public PaymentReceiptDTO PaymentReceipt { get; set; }
    }
}
