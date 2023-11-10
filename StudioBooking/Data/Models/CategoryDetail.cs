using Microsoft.EntityFrameworkCore;
using StudioBooking.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudioBooking.Data.Models
{
    public class CategoryDetail : BaseEntity
    {
        public int Id { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public int Type { get; set; }
        [MaxLength(250)]
        public string Title { get; set; }
        public string? Description { get; set; }
        public Category Category { get; set; }
        public ICollection<CategoryGear> Gears { get; set; }

        public static async Task DeleteCategoryDetail(ApplicationDbContext context, int categoryId, string userId)
        {
            var categoryDetails = await context.CategoryDetails.Where(s => s.CategoryId == categoryId && s.IsActive).ToListAsync();
            foreach (var categoryDetail in categoryDetails)
            {
                categoryDetail.IsActive = false;
                categoryDetail.IsDelete = true;
                categoryDetail.ModifiedBy = userId;
                categoryDetail.ModifiedDate = Defaults.GetDateTime();
                await CategoryGear.DeleteCategoryDetailGears(context,categoryDetail.Id,userId);
            }
        }
    }
}
