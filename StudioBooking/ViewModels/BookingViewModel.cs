using StudioBooking.Data.Models;
using StudioBooking.Data;
using StudioBooking.DTO;
using Microsoft.EntityFrameworkCore;
using StudioBooking.Infrastructure;
using CCAvenue;
using static StudioBooking.Infrastructure.Enums;

namespace StudioBooking.ViewModels
{
    public class BookingViewModel
    {
        public ServicePriceDTO? ServicePrice { get; set; }
        public BookingDTO Booking { get; set; }
        public CartDTO? Cart { get; set; }
        public CustomerDTO? Customer { get; set; }
        public bool DifferentBillingAddress { get; set; }
        public long ScheduleRequestId { get; set; }
        public CustomerAddressDTO? CustomerAddress { get; set; }
        public List<CustomerAddressDTO> CustomerAddresses { get; set; }
        public List<ServiceDTO> Services { get; set; }
        public List<ServicePriceDTO> ServicePrices { get; set; }
        public List<AddOnDTO> Addons { get; set; }
        public List<WalletDTO> Wallets { get; set; }
        public List<TransactionDTO> Transactions { get; set; }
        public BookingViewModel()
        {
            ServicePrice = new ServicePriceDTO();
            Booking = new BookingDTO();
            Cart = new CartDTO();
            Customer = new CustomerDTO();
            Services = new List<ServiceDTO>();
            ServicePrices = new List<ServicePriceDTO>();
            Addons = new List<AddOnDTO>();
            Wallets = new List<WalletDTO>();
            Transactions = new List<TransactionDTO>();
        }

        public static Task<List<BookingViewModel>> GetCustomerBookings(ApplicationDbContext context, string userId)
        {
            return context.Bookings.Include(b => b.Transactions.Where(t => t.Status == (int)Enums.TransactionStatus.Success)).Include(c => c.Customer).Include(s => s.ServicePrice).Include(s => s.ServicePrice.Category).Include(s => s.ServicePrice.Service).Include(b => b.ScheduleRequests).Where(b => b.IsActive && !b.IsDelete && b.Customer.UserId == userId && (b.BookingStatus != (int)BookingStatus.Failed)).Select(b => new BookingViewModel
            {
                Booking = BookingDTO.GetBooking(b),
                Customer = CustomerDTO.GetCustomer(b.Customer ?? new Customer()),
                ServicePrice = ServicePriceDTO.GetServicePrice(b.ServicePrice ?? new ServicePrice()),
                Transactions = b.Transactions.Where(t => t.Status == (int)Enums.TransactionStatus.Success).Select(t => new TransactionDTO { Id = t.Id, CustomerId = t.CustomerId, PaymentMode = (Enums.PaymentMode)t.PaymentMode }).ToList()
            }).ToListAsync();
        }
    }
}
