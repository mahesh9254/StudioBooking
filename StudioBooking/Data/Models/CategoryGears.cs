using Microsoft.EntityFrameworkCore;
using StudioBooking.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudioBooking.Data.Models
{
    public class CategoryGear : BaseEntity
    {
        public int Id { get; set; }
        [ForeignKey("CategoryDetail")]
        public int CategoryDetailId { get; set; }
        [MaxLength(250)]
        public string Content { get; set; }
        public int? Type { get; set; }
        [MaxLength(50)]
        public string? Icon { get; set; }
        [MaxLength(200)]
        public string? Image { get; set; }
        public CategoryDetail CategoryDetail { get; set; }

        public static async Task DeleteCategoryDetailGears(ApplicationDbContext context, int categoryDetailId, string userId)
        {
            var categoryGears = await context.CategoryGears.Where(s => s.CategoryDetailId == categoryDetailId && s.IsActive).ToListAsync();
            foreach (var categoryGear in categoryGears)
            {
                categoryGear.IsActive = false;
                categoryGear.IsDelete = true;
                categoryGear.ModifiedBy = userId;
                categoryGear.ModifiedDate = Defaults.GetDateTime();
            }
        }
    }
}
