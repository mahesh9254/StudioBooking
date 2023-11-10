
using Microsoft.EntityFrameworkCore;
using StudioBooking.Data;
using StudioBooking.Data.Models;
using StudioBooking.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace StudioBooking.DTO
{
    public class CustomerAddressDTO
    {
        public long Id { get; set; }
        public long? CustomerId { get; set; }
        public string? Name { get; set; }
        public string? CompanyName { get; set; }
        public string? GstNumber { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? Landmark { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        [StringLength(6)]
        [MinLength(6)]
        public string? PinCode { get; set; }
        public bool IsDefault { get; set; }
        public bool IsSelected { get; set; }

        public static async Task<CustomerAddressDTO> GetAddress(ApplicationDbContext context, long id)
        {
            var address = await context.CustomerAddresses.FirstOrDefaultAsync(c => c.Id == id && c.IsActive) ?? new CustomerAddress();
            return new CustomerAddressDTO
            {
                Id = address.Id,
                CustomerId = address.CustomerId,
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                Landmark = address.Landmark,
                City = address.City,
                State = address.State,
                CompanyName = address.CompanyName,
                Name = address.Name,
                GstNumber = address.GstNumber,
                IsDefault = address.IsDefault,
                PinCode = address.PinCode,
            };
        }

        public static async Task<List<CustomerAddressDTO>> GetAddressByCustomerId(ApplicationDbContext context, long customerId)
        {
            return await context.CustomerAddresses.Where(c => c.CustomerId == customerId && c.IsActive).Select(s => new CustomerAddressDTO
            {
                Id = s.Id,
                CustomerId = s.CustomerId,
                AddressLine1 = s.AddressLine1,
                AddressLine2 = s.AddressLine2,
                Landmark = s.Landmark,
                City = s.City,
                State = s.State,
                CompanyName = s.CompanyName,
                Name = s.Name,
                GstNumber = s.GstNumber,
                IsDefault = s.IsDefault,
                PinCode = s.PinCode,
            }).ToListAsync();
        }

        public static async Task<CustomerAddressDTO> SaveAddressAsync(ApplicationDbContext context, long customerId, CustomerAddressDTO customerAddress)
        {
            var newAddress = new CustomerAddress
            {
                CustomerId = customerId,
                Name = customerAddress.Name,
                CompanyName = customerAddress.CompanyName,
                GstNumber = customerAddress.GstNumber,
                AddressLine1 = customerAddress.AddressLine1,
                AddressLine2 = customerAddress.AddressLine2,
                Landmark = customerAddress.Landmark,
                City = customerAddress.City,
                State = customerAddress.State,
                IsActive=true,
                IsDelete=false,
                PinCode = customerAddress.PinCode == null ? null : customerAddress.PinCode?.ToString().Substring(0, 5),
                CreatedDate = Defaults.GetDateTime()
            };
            await context.CustomerAddresses.AddAsync(newAddress);
            await context.SaveChangesAsync();
            customerAddress.Id = newAddress.Id;
            return customerAddress;
        }

        public static string GetAddress(CustomerAddressDTO? customerAddress)
        {
            if (customerAddress == null) return string.Empty;
            var address = customerAddress.AddressLine1;
            if (!string.IsNullOrEmpty(customerAddress.AddressLine2))
                address += "," + customerAddress.AddressLine2;
            if (!string.IsNullOrEmpty(customerAddress.Landmark))
                address += "," + customerAddress.Landmark;
            return address;
        }
    }
}
