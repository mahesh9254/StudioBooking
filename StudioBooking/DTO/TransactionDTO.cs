
using Microsoft.EntityFrameworkCore;
using StudioBooking.Data;
using StudioBooking.Infrastructure;
using static StudioBooking.Infrastructure.Enums;

namespace StudioBooking.DTO
{
	public class TransactionDTO
	{
		public long Id { get; set; }
		public long? BookingId { get; set; }
		public long? CustomerId { get; set; }
		public PaymentMode PaymentMode { get; set; }
		public TransactionType TransactionType { get; set; }
		public PaymentType PaymentType { get; set; }
		public double Amount { get; set; }
		public double TotalAmount { get; set; }
		public TransactionStatus Status { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public string CreatedBy { get; set; }
		public string CreatedDate { get; set; }

		public static async Task<List<TransactionDTO>> GetUserTransactions(ApplicationDbContext context, string userId, bool getRecent = false)
		{
			return await context.Transactions.Include(t => t.Customer).Where(t => t.Customer.UserId == userId && !t.IsDelete && t.IsActive && t.CreatedDate >= (getRecent ? DateTime.UtcNow.AddDays(-10) : Defaults.GetLegecyDate()).Date).Select(t => new TransactionDTO
			{
				Id = t.Id,
				BookingId = t.BookingId,
				CustomerId = t.CustomerId,
				PaymentMode = (PaymentMode)t.PaymentMode,
				PaymentType = (PaymentType)t.PaymentType,
				Amount = t.Amount,
				TransactionType = (TransactionType)t.TransactionType,
				Status = (TransactionStatus)t.Status,
				CreatedDate = t.CreatedDate.ToString("dd-MMM-yyyy hh:mm:tt")
			}).ToListAsync();
		}

		public static async Task<List<TransactionDTO>> GetTransactionDetail(ApplicationDbContext context, string userId, string id)
		{
			return await context.Transactions.Include(t => t.Customer).Include(t => t.Booking).Include(b => b.PaymentGatewayResponses).Include(b => b.PaymentReceipts).Where(t => t.Id.ToString() == userId && !t.IsDelete && t.IsActive).Select(t => new TransactionDTO
			{
				Id = t.Id,
				BookingId = t.BookingId,
				CustomerId = t.CustomerId,
				PaymentMode = (PaymentMode)t.PaymentMode,
				PaymentType = (PaymentType)t.PaymentType,
				Amount = t.Amount,
				TransactionType = (TransactionType)t.TransactionType,
				Status = (TransactionStatus)t.Status,
				CreatedDate = t.CreatedDate.ToString("dd-MMM-yyyy hh:mm:tt")
			}).ToListAsync();
		}
	}
}
