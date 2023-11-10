using StudioBooking.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudioBooking.Data.Models
{
    public class Transaction : BaseEntity
    {
        public long Id { get; set; }
        [ForeignKey("Booking")]
        public long? BookingId { get; set; }
        [ForeignKey("Customer")]
        public long? CustomerId { get; set; }
        public int PaymentMode { get; set; }
        public int TransactionType { get; set; }
        public int PaymentProvider { get; set; }
        public int PaymentType { get; set; }
        public double Amount { get; set; }
        public int Status { get; set; }
        public Booking? Booking { get; set; }
        public Customer? Customer { get; set; }
        public ICollection<PaymentGatewayResponse> PaymentGatewayResponses { get; set; }
        public ICollection<PaymentReceipt> PaymentReceipts { get; set; }
        public Transaction()
        {
            PaymentGatewayResponses= new List<PaymentGatewayResponse>();
            PaymentReceipts= new List<PaymentReceipt>();
        }

        public static Transaction SetTransaction(Booking booking, string userId)
        {
            return new Transaction
            {
                BookingId = booking.Id,
                PaymentType = booking.PaymentStatus,
                Status = (int)Enums.TransactionStatus.Pending,
                TransactionType = (int)Enums.TransactionType.Debit,
                Amount = booking.PaymentStatus == (int)Enums.PaymentStatus.Advance ? booking.Total / 2 : booking.Total,
                CustomerId = booking.CustomerId,
                IsActive = true,
                CreatedBy = userId,
                CreatedDate = Defaults.GetDateTime(),
                ModifiedBy = userId,
                ModifiedDate = Defaults.GetDateTime()
            };
        }
    }
}
