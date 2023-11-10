using Microsoft.EntityFrameworkCore;
using StudioBooking.Data;
using StudioBooking.Data.Models;
using StudioBooking.DTO;
using StudioBooking.Infrastructure;
using static StudioBooking.Infrastructure.Enums;

namespace StudioBooking.ViewModels
{
    public class PaymentReceiptViewModel
    {
        public TransactionDTO Transaction { get; set; }
        public BookingDTO Booking { get; set; }
        public CustomerDTO Customer { get; set; }
        public List<AddOnDTO> AddOns { get; set; }
        public List<TransactionViewModel> Transactions { get; set; }
        public PaymentReceiptViewModel()
        {
            AddOns = new List<AddOnDTO>();
        }


        public static async Task<PaymentReceiptViewModel> GetPaymentReceipt(ApplicationDbContext context, long bookingId)
        {
            var booking = await context.Bookings.Include(t => t.Customer.CustomerAddresses).Include(t => t.Customer.User).FirstOrDefaultAsync(b => b.Id == bookingId);
            var transactions = await context.Transactions.Include(t => t.PaymentGatewayResponses).Include(t => t.PaymentReceipts).Where(t => t.BookingId == booking.Id && t.Status == (int)TransactionStatus.Success).ToListAsync();
            if (!transactions.Any()) throw new ArgumentNullException("GetPaymentReceipt");

            var customerAddress = booking.Customer.CustomerAddresses.FirstOrDefault(s => s.Id == booking.BillingAddressId);

            var totalPaid = transactions.Sum(t => t.Amount);
            if (transactions.Any(t => t.PaymentMode == (int)Enums.PaymentMode.Online || t.PaymentMode == (int)(Enums.PaymentMode.Wallet | Enums.PaymentMode.Online)))
            {
                totalPaid += await WalletDTO.GetBookingPaymentsFromWalletAsync(context, transactions);
            }
            var serviceDetail = await ServicePriceDTO.GetServicePrice(context, booking.ServicePriceId);
            var bookingDetail = BookingDTO.GetBooking(booking);
            bookingDetail.AdvancePaid = totalPaid;
            bookingDetail.ServiceName = serviceDetail.ServiceName + " - " + serviceDetail.CategoryName;
            var walletTransactions = await context.WalletTransactions.Where(w => w.ReferenceId == bookingDetail.BookingId && w.TransactionType == (int)TransactionType.Debit).ToListAsync();
            return new PaymentReceiptViewModel
            {
                Booking = bookingDetail,
                Customer = new CustomerDTO
                {
                    Id = booking.Customer.Id,
                    AddressLine1 = customerAddress.AddressLine1,
                    AddressLine2 = customerAddress.AddressLine2,
                    City = customerAddress.City,
                    CompanyName = customerAddress.CompanyName,
                    GstNumber = customerAddress.GstNumber,
                    Landmark = customerAddress.Landmark,
                    Name = customerAddress.Name,
                    PinCode = customerAddress.PinCode,
                    State = customerAddress.State,
                    UserId = customerAddress.Customer.UserId,
                    Mobile = booking.Customer.User != null ? booking.Customer.User.PhoneNumber : null,
                },
                AddOns = booking.IsAddonRequested ? await AddOnDTO.GetBookingAddOns(context, booking.Id) : new List<AddOnDTO>(),
                Transactions = transactions.Select(t => new TransactionViewModel
                {
                    Transaction = new TransactionDTO
                    {
                        Id = t.Id,
                        Amount = t.Amount,
                        BookingId = booking.Id,
                        PaymentMode = (PaymentMode)t.PaymentMode,
                        Status = (TransactionStatus)t.Status
                    },
                    PaymentGatewayResponse = t.PaymentGatewayResponses.Select(p => new PaymentGatewayResponse
                    {
                        TransactionId = p.TransactionId,
                        BookingId = t.BookingId,
                        OrderId = p.OrderId,
                        OrderAmount = p.OrderAmount,
                        OrderCurrency = p.OrderCurrency,
                        BankRefNo = p.BankRefNo,
                        PaymentSessionId = p.PaymentSessionId,
                        CreatedDate = p.CreatedDate
                    }).FirstOrDefault() ?? new PaymentGatewayResponse(),
                    PaymentReceipt = t.PaymentReceipts.Select(p => new PaymentReceiptDTO
                    {

                    }).FirstOrDefault() ?? new PaymentReceiptDTO(),
                    WalletTransaction = walletTransactions.Where(w => w.Description == t.Id.ToString(Defaults.TransactionPrefix)).OrderByDescending(w => w.Id).Select(w => new WalletTransactionDTO
                    {
                        Id = w.Id,
                        Amount = w.Amount,
                        Description = w.Description,
                        Hours = w.Hours,
                        TransactionType = (TransactionType)w.TransactionType,
                        ReferenceId = w.ReferenceId,
                        Mode = (WalletType)w.Mode,
                        WalletId = w.WalletId
                    }).FirstOrDefault() ?? new WalletTransactionDTO()
                }).ToList(),
            };
        }

        //public static async Task<PaymentReceiptViewModel> GetPaymentReceipt(ApplicationDbContext context, long transactionId)
        //{
        //    var transaction = await context.Transactions.Include(t => t.Booking).Include(t => t.PaymentGatewayResponses).Include(t => t.PaymentReceipts).Include(t => t.Customer.CustomerAddresses).Include(t => t.Customer.User).FirstOrDefaultAsync(t => t.Id == transactionId);
        //    if (transaction == null) throw new ArgumentNullException(nameof(transaction));
        //    var customerAddress = transaction.Customer.CustomerAddresses.FirstOrDefault(s => s.Id == transaction.Booking.BillingAddressId);
        //    var paymentGatewayResponse = transaction.PaymentGatewayResponses.FirstOrDefault(s => s.OrderStatus == (transaction.PaymentProvider == (int)PaymentProvider.CCAvenue ? (int)CCAvenue.OrderStatus.Success : (int)Cashfree.OrderStatus.PAID));
        //    var totalPaid = transaction.Amount;
        //    if (transaction.PaymentMode == (int)Enums.PaymentMode.Online || transaction.PaymentMode == (int)(Enums.PaymentMode.Wallet | Enums.PaymentMode.Online))
        //    {
        //        totalPaid += await WalletDTO.GetTransactionPaymentsFromWalletAsync(context, new List<Transaction> { transaction });
        //    }
        //    var serviceDetail = await ServicePriceDTO.GetServicePrice(context, transaction.Booking.ServicePriceId);
        //    var booking = BookingDTO.GetBooking(transaction.Booking);
        //    booking.AdvancePaid = totalPaid;
        //    booking.ServiceName = serviceDetail.ServiceName + " - " + serviceDetail.CategoryName;

        //    return new PaymentReceiptViewModel
        //    {
        //        Booking = booking,
        //        Customer = new CustomerDTO
        //        {
        //            Id = transaction.Customer.Id,
        //            AddressLine1 = customerAddress.AddressLine1,
        //            AddressLine2 = customerAddress.AddressLine2,
        //            City = customerAddress.City,
        //            CompanyName = customerAddress.CompanyName,
        //            GstNumber = customerAddress.GstNumber,
        //            Landmark = customerAddress.Landmark,
        //            Name = customerAddress.Name,
        //            PinCode = customerAddress.PinCode,
        //            State = customerAddress.State,
        //            UserId = customerAddress.Customer.UserId,
        //            Mobile = transaction.Customer.User != null ? transaction.Customer.User.PhoneNumber : null,
        //        },
        //        AddOns = booking.IsAddonRequested ? await AddOnDTO.GetBookingAddOns(context,booking.Id): new List<AddOnDTO>(),
        //        PaymentGatewayResponse = paymentGatewayResponse ?? new PaymentGatewayResponse(),
        //        Transaction = new TransactionDTO
        //        {
        //            Id = transaction.Id,
        //            Amount = transaction.Amount,
        //            PaymentMode = (Enums.PaymentMode)transaction.PaymentMode,
        //            CreatedDate = transaction.CreatedDate.ToString(Defaults.DefaultDateFormat)
        //        }
        //    };
        //}
    }
}
