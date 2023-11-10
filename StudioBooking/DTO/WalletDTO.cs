using Microsoft.EntityFrameworkCore;
using StudioBooking.Data;
using StudioBooking.Data.Models;
using StudioBooking.Infrastructure;
using static StudioBooking.Infrastructure.Enums;

namespace StudioBooking.DTO
{
    public class WalletDTO
    {
        public long Id { get; set; }
        public long? CustomerId { get; set; }
        public WalletType WalletType { get; set; }
        public int Mode { get; set; }
        public double? BalanceAmount { get; set; }
        public double? BalanceHours { get; set; }
        public double? Amount { get; set; }
        public int TransactionType { get; set; }
        public string? Description { get; set; }

        public static async Task<List<WalletDTO>> GetCustomerWallets(ApplicationDbContext context, long id)
        {
            return await context.Wallets.Where(w => w.CustomerId == id && w.IsActive).Select(w => new WalletDTO
            {
                Id = w.Id,
                CustomerId = w.CustomerId,
                Mode = w.Mode,
                WalletType = (WalletType)w.WalletType,
                BalanceAmount = w.BalanceAmount,
                BalanceHours = w.BalanceHours
            }).ToListAsync();
        }

        public static async Task<bool> AddUpdateWallet(ILogger logger, ApplicationDbContext context, Wallet wallet, WalletTransactionDTO walletTransaction, string userId)
        {
            try
            {
                var transaction = new WalletTransaction
                {
                    WalletId = wallet.Id,
                    Amount = walletTransaction.Amount,
                    Hours = walletTransaction.Hours,
                    TransactionType = (int)walletTransaction.TransactionType,
                    Mode = wallet.Mode,
                    Description = walletTransaction.Description,
                    IsActive = true,
                    ReferenceId = walletTransaction.ReferenceId,
                    CreatedBy = userId,
                    CreatedDate = Defaults.GetDateTime()
                };
                await context.WalletTransactions.AddAsync(transaction);
                if (wallet.Mode == (int)WalletType.Points)
                {
                    wallet.BalanceAmount = walletTransaction.TransactionType == Enums.TransactionType.Debit ? Math.Round((wallet.BalanceAmount - walletTransaction.Amount) ?? 0, 2) : Math.Round((wallet.BalanceAmount + walletTransaction.Amount) ?? 0, 2);
                }
                else
                {
                    wallet.BalanceHours = walletTransaction.TransactionType == Enums.TransactionType.Debit ? Math.Round((wallet.BalanceHours - walletTransaction.Hours) ?? 0, 2) : Math.Round((wallet.BalanceHours + walletTransaction.Hours) ?? 0, 2);
                }
                wallet.ModifiedBy = userId;
                wallet.ModifiedDate = Defaults.GetDateTime();
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error when AddUpdateWallet");
                return await Task.FromResult(false);
            }
        }

        public static async Task<double> GetBookingPaymentsFromWalletAsync(ApplicationDbContext context, List<Transaction> transactions)
        {
            var walletPayments = transactions.Where(t => t.PaymentMode == (int)Enums.PaymentMode.Online || t.PaymentMode == (int)(Enums.PaymentMode.Wallet | Enums.PaymentMode.Online)).Select(s => (s.BookingId ?? 0).ToString(Defaults.BookingPrefix));
            var walletTransactions = await context.WalletTransactions.Where(s => walletPayments.Contains(s.ReferenceId) && s.IsActive).ToListAsync();
            var debitTransactions = walletTransactions.Where(s => s.TransactionType == (int)Enums.TransactionType.Debit).Sum(s => s.Amount ?? 0);
            var creditTransactions = walletTransactions.Where(s => s.TransactionType == (int)Enums.TransactionType.Credit).Sum(s => s.Amount ?? 0);
            return (debitTransactions - creditTransactions);
        }

        public static async Task<double> GetTransactionPaymentsFromWalletAsync(ApplicationDbContext context, List<Transaction> transactions)
        {
            var walletPayments = transactions.Where(t => t.PaymentMode == (int)Enums.PaymentMode.Online || t.PaymentMode == (int)(Enums.PaymentMode.Wallet | Enums.PaymentMode.Online)).Select(s => s.Id.ToString(Defaults.TransactionPrefix));
            var walletTransactions = await context.WalletTransactions.Where(s => walletPayments.Contains(s.Description) && s.IsActive).ToListAsync();
            var debitTransactions = walletTransactions.Where(s => s.TransactionType == (int)Enums.TransactionType.Debit).Sum(s => s.Amount ?? 0);
            var creditTransactions = walletTransactions.Where(s => s.TransactionType == (int)Enums.TransactionType.Credit).Sum(s => s.Amount ?? 0);
            return (debitTransactions - creditTransactions);
        }
    }
}
