using Microsoft.EntityFrameworkCore;
using StudioBooking.Data;
using StudioBooking.Data.Models;
using StudioBooking.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;
using static StudioBooking.Infrastructure.Enums;

namespace StudioBooking.DTO
{
    public class CategoryGearDTO
    {
        public int Id { get; set; }
        public int CategoryDetailId { get; set; }
        public string Content { get; set; }
        public GearContentType? Type { get; set; }
        public string? Icon { get; set; }
        public string? Image { get; set; }
        public IFormFile? ImageFile { get; set; }
        public CategoryGearDTO()
        {
            Type = GearContentType.Basic;
        }

        public static async void SaveGearDetails(ApplicationDbContext context, CategoryDetail categoryDetail, List<CategoryGearDTO> categoryGears)
        {
            foreach (var gear in categoryGears)
            {
                var gearInDb = await context.CategoryGears.FirstOrDefaultAsync(c => c.Id == gear.Id && !c.IsDelete);
                if (gearInDb == null)
                {
                    gearInDb = new CategoryGear
                    {
                        CategoryDetailId = categoryDetail.Id,
                        Content = gear.Content,
                        IsActive = true,
                        Type = (int)(gear.Type ?? 0),
                        Icon = gear.Icon,
                        Image = gear.Image,
                        CreatedBy = categoryDetail.ModifiedBy,
                        CreatedDate = Defaults.GetDateTime(),
                        ModifiedBy = categoryDetail.ModifiedBy,
                        ModifiedDate = Defaults.GetDateTime()
                    };
                    await context.CategoryGears.AddAsync(gearInDb);
                }
                else
                {
                    gearInDb.CategoryDetailId = categoryDetail.Id;
                    gearInDb.Content = gear.Content;
                    gearInDb.Type = (int)(gear.Type ?? 0);
                    gearInDb.Icon = gear.Icon;
                    gearInDb.Image = gear.Image;
                    gearInDb.ModifiedBy = categoryDetail.ModifiedBy;
                    gearInDb.ModifiedDate = Defaults.GetDateTime();                    
                }
                await context.SaveChangesAsync();
                gear.Id = gearInDb.Id;
            }
        }
    }
}
