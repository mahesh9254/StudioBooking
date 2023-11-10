using Microsoft.EntityFrameworkCore;
using StudioBooking.Areas.Admin.Models;
using StudioBooking.DTO;
using StudioBooking.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudioBooking.Data.Models
{
    public class Customer : BaseEntity
    {
        public long Id { get; set; }
        [ForeignKey("User")]
        public string? UserId { get; set; }
        public string? Name { get; set; }

        [StringLength(200)]
        public string? CompanyName { get; set; }        
        public ApplicationUser? User { get; set; }
        public ICollection<Cart> Carts { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<Wallet> Wallets { get; set; }
        public ICollection<CustomerAddress> CustomerAddresses { get; set; }

        public Customer()
        {
            Carts = new List<Cart>();
            Bookings = new List<Booking>();
            Transactions = new List<Transaction>();
            Wallets = new List<Wallet>();
            CustomerAddresses = new List<CustomerAddress>();
        }

        public static async Task<Customer> GetCustomerByUserId(ApplicationDbContext context, string userId)
        {
            return await context.Customers.FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive && !c.IsDelete) ?? new Customer();
        }

        public static async Task<Customer> AddCustomer(ApplicationDbContext context, ApplicationUser user, CustomerDTO customerDTO, string userId)
        {
            var customer = new Customer
            {
                UserId = user.Id,
                Name = user.FirstName + " " + user.LastName,
                CompanyName = customerDTO.CompanyName,
                //GstNumber = customerDTO.GstNumber,
                //AddressLine1 = customerDTO.AddressLine1,
                //AddressLine2 = customerDTO.AddressLine2,
                //Landmark = customerDTO.Landmark,
                //City = customerDTO.City,
                //State = customerDTO.State,
                //PinCode = customerDTO.PinCode,
                IsActive = true,
                CreatedBy = userId,
                CreatedDate = Defaults.GetDateTime(),
                ModifiedBy = userId,
                ModifiedDate = Defaults.GetDateTime()
            };
            await context.Customers.AddAsync(customer);
            await context.SaveChangesAsync();
            return customer;
        }

    }
}
