using Microsoft.EntityFrameworkCore;
using StudioBooking.Data;
using StudioBooking.Infrastructure;

namespace StudioBooking.DTO
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public IFormFile? ImageName { get; set; }
        public bool IsActive { get; set; }
        public bool? DisableBooking { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ModifiedDate { get; set; }

        public static async Task<CategoryDTO> GetCategory(ApplicationDbContext dbContext, int id)
        {
            var categoryInDb = await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id && !c.IsDelete); 
            return new CategoryDTO
            {
                Id = id,
                Name = categoryInDb.Name,
                Title = categoryInDb.Title,
                Description = categoryInDb.Description,
                Image = categoryInDb.Image,
                StartTime = categoryInDb.StartTime,
                EndTime = categoryInDb.EndTime,
                IsActive = categoryInDb.IsActive
            };
        }
    }
}
