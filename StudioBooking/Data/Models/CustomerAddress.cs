using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudioBooking.Data.Models
{
    public class CustomerAddress : BaseEntity
    {
        public long Id { get; set; }
        [ForeignKey("Customer")]
        public long? CustomerId { get; set; }
        [StringLength(200)]
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
        [StringLength(50)]
        public string? State { get; set; }

        [StringLength(6)]
        public string? PinCode { get; set; }
        public bool IsDefault { get; set; }
        public Customer Customer { get; set; }

        public static async Task AddAddress(ApplicationDbContext context, CustomerAddress customerAddress)
        {
           await context.CustomerAddresses.AddAsync(customerAddress);
        }
    }
}
