using Microsoft.EntityFrameworkCore;
using StudioBooking.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudioBooking.Data.Models
{
    public class ServicePrice : BaseEntity
    {
        public int Id { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        [ForeignKey("Service")]
        public int ServiceId { get; set; }
        public float Price { get; set; }
        public int MinHours { get; set; }
        [MaxLength(20)]
        public string? EventColorId { get; set; }
        [MaxLength(200)]
        public string? CalenderName { get; set; }
        public string? Description { get; set; }
        public string? AdditionalInformation { get; set; }
        public bool? DisableBooking { get; set; }
        public Category Category { get; set; }
        public Service Service { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public ServicePrice()
        {
            Category = new Category();
            Service = new Service();
            Bookings = new List<Booking>();
        }

        public static async Task DeleteCategoryServicePrices(ApplicationDbContext context, int categoryId, string userId)
        {
            var servicePrices = await context.ServicePrices.Where(s => s.CategoryId == categoryId && s.IsActive).ToListAsync();
            foreach (var servicePrice in servicePrices)
            {
                servicePrice.IsActive = false;
                servicePrice.IsDelete = true;
                servicePrice.ModifiedBy = userId;
                servicePrice.ModifiedDate = Defaults.GetDateTime();
            }
        }
    }
}
