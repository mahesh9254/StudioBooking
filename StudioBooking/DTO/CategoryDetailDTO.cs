using Microsoft.EntityFrameworkCore;
using StudioBooking.Data;
using StudioBooking.Data.Models;
using StudioBooking.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using static StudioBooking.Infrastructure.Enums;

namespace StudioBooking.DTO
{
    public class CategoryDetailDTO
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryImage { get; set; }
        public DetailType Type { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public List<CategoryGearDTO> CategoryGears { get; set; }
        public CategoryDetailDTO()
        {
            CategoryGears = new List<CategoryGearDTO>();
        }

        public static async Task<List<CategoryDetailDTO>> GetCategoryDetails(ApplicationDbContext context)
        {
            return await context.CategoryDetails.Include(c => c.Category).Where(c => c.IsActive && !c.IsDelete).Select(s => new CategoryDetailDTO
            {
                CategoryId = s.CategoryId,
                Id = s.Id,
                Title = s.Title,
                CategoryName = s.Category.Name,
                CategoryImage = s.Category.Image,
                Type = (DetailType)s.Type,
                Description = s.Description,
            }).ToListAsync();
        }

        public static async Task<List<CategoryDetailDTO>> GetCategoryDetails(ApplicationDbContext context, int categoryId)
        {
            return await context.CategoryDetails.Include(c => c.Gears).Where(c => c.CategoryId == categoryId && c.IsActive && !c.IsDelete).Select(s => new CategoryDetailDTO
            {
                CategoryId = s.CategoryId,
                Id = s.Id,
                Title = s.Title,
                Type = (DetailType)s.Type,
                Description = s.Description,
                CategoryGears = s.Gears.Where(m => m.IsActive && !m.IsDelete).Select(g => new CategoryGearDTO
                {
                    Id = g.Id,
                    CategoryDetailId = g.CategoryDetailId,
                    Content = g.Content,
                    Icon = g.Icon,
                    Image = g.Image,
                    Type = (GearContentType)(g.Type ?? 0)
                }).ToList()
            }).ToListAsync();
        }

        public static async Task<CategoryDetail> SaveCategoryDetail(ApplicationDbContext context, CategoryDetailDTO categoryDetail, string userId)
        {
            var newCategoryDetail = new CategoryDetail();
            var categoryDetailIndb = await context.CategoryDetails.FirstOrDefaultAsync(c => c.Id == categoryDetail.Id && !c.IsDelete);
            if (categoryDetailIndb == null)
            {
                newCategoryDetail = new CategoryDetail
                {
                    Title = categoryDetail.Title,
                    Description = categoryDetail.Description,
                    CategoryId = categoryDetail.CategoryId,
                    Type = (int)categoryDetail.Type,
                    IsActive = true,
                    CreatedBy = userId,
                    CreatedDate = Defaults.GetDateTime(),
                    ModifiedBy = userId,
                    ModifiedDate = Defaults.GetDateTime()
                };
                await context.CategoryDetails.AddAsync(newCategoryDetail);
            }
            else
            {
                categoryDetailIndb.Title = categoryDetail.Title;
                categoryDetailIndb.Description = categoryDetail.Description;
                categoryDetailIndb.CategoryId = categoryDetail.CategoryId;
                categoryDetailIndb.Type = (int)categoryDetail.Type;
                categoryDetailIndb.ModifiedBy = userId;
                categoryDetailIndb.ModifiedDate = Defaults.GetDateTime();
                newCategoryDetail = categoryDetailIndb;
            }
            await context.SaveChangesAsync();
            categoryDetail.Id = newCategoryDetail.Id;
            return newCategoryDetail;
        }
    }
}
