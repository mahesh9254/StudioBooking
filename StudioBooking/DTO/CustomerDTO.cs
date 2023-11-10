using Microsoft.EntityFrameworkCore;
using StudioBooking.Data;
using StudioBooking.Data.Models;
using StudioBooking.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace StudioBooking.DTO
{
    public class CustomerDTO
    {
        public long Id { get; set; }
        public string? UserId { get; set; }
        public string? Name { get; set; }

        [StringLength(200)]
        public string? CompanyName { get; set; }

        [StringLength(15)]
        public string? GstNumber { get; set; }

        [StringLength(50)]
        public string? AddressLine1 { get; set; }

        [StringLength(50)]
        public string? AddressLine2 { get; set; }

        [StringLength(50)]
        public string? Landmark { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        public string? State { get; set; }

        [StringLength(6)]
        public string? PinCode { get; set; }
        public string? Mobile { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public static async Task<CustomerDTO> GetCustomer(ApplicationDbContext context, string userId)
        {
            var customer = await context.Customers.Include(c => c.User).Include(c=> c.CustomerAddresses.Where(c=> c.IsDefault)).FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive && c.IsDelete != true);
            return new CustomerDTO
            {
                Id = customer.Id,
                UserId = customer.UserId,
                Name = customer.Name,
                CompanyName = customer.CompanyName,
                Mobile = customer.User != null ? customer.User.PhoneNumber : null,
                GstNumber = customer.CustomerAddresses.FirstOrDefault().GstNumber,
                AddressLine1 = customer.CustomerAddresses.FirstOrDefault().AddressLine1,
                AddressLine2 = customer.CustomerAddresses.FirstOrDefault().AddressLine2,
                Landmark = customer.CustomerAddresses.FirstOrDefault().Landmark,
                City = customer.CustomerAddresses.FirstOrDefault().City,
                State = customer.CustomerAddresses.FirstOrDefault().State,
                PinCode = customer.CustomerAddresses.FirstOrDefault().PinCode,
                CreatedBy = customer.CreatedBy,
                CreatedDate = Defaults.GetDateTime()
            };
        }

        public static async Task<List<CustomerDTO>> GetCustomers(ApplicationDbContext context)
        {
            return await context.Customers.Include(c => c.User).Where(c => c.IsActive && c.IsDelete != true).Select(c => new CustomerDTO
            {
                Id = c.Id,
                UserId = c.UserId,
                Name = c.Name,
                CompanyName = c.CompanyName,
                Mobile = c.User != null ? c.User.PhoneNumber : null,
                //GstNumber = c.GstNumber,
                //AddressLine1 = c.AddressLine1,
                //AddressLine2 = c.AddressLine2,
                //Landmark = c.Landmark,
                //City = c.City,
                //State = c.State,
                //PinCode = c.PinCode,
                CreatedBy = c.CreatedBy,
                CreatedDate = Defaults.GetDateTime()
            }).ToListAsync();
        }

        public static CustomerDTO GetCustomer(Customer customer)
        {
            return new CustomerDTO
            {
                UserId = customer.UserId,
                Name = customer.Name,
                CompanyName = customer.CompanyName,
                Mobile = customer.User != null ? customer.User.PhoneNumber : null,
                //GstNumber = customer.GstNumber,
                //AddressLine1 = customer.AddressLine1,
                //AddressLine2 = customer.AddressLine2,
                //Landmark = customer.Landmark,
                //City = customer.City,
                //State = customer.State,
                //PinCode = customer.PinCode,
                CreatedBy = customer.CreatedBy
            };
        }
    }
}
