using Microsoft.EntityFrameworkCore;
using StudioBooking.Data;
using StudioBooking.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace StudioBooking.DTO
{
    public class ServiceGalleryDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        [MaxLength(250)]
        public string Title { get; set; }
        [MaxLength(250)]
        public string? SubTitle { get; set; }
        [MaxLength(250)]
        public string? Link { get; set; }
        [MaxLength(500)]
        public string? Image { get; set; }
        public IFormFile? ImageName { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedDate { get; set; }
        public static async Task<ServiceGalleryDTO> GetServiceGallery(ApplicationDbContext dbContext, int id)
        {
            var serviceGallery = await dbContext.ServiceGallery.FirstOrDefaultAsync(c => c.Id == id && !c.IsDelete);
            return new ServiceGalleryDTO
            {
                Id = serviceGallery.Id,
                OrderId = serviceGallery.OrderId,
                Title = serviceGallery.Title,
                SubTitle = serviceGallery.SubTitle,
                Link = serviceGallery.Link,
                Image = serviceGallery.Image,
                IsActive = serviceGallery.IsActive,
                CreatedBy = serviceGallery.CreatedBy,
                CreatedDate = serviceGallery.CreatedDate.ToShortDateString()
            };
        }

        public static IEnumerable<ServiceGalleryDTO> GetServiceGalleries(ApplicationDbContext context)
        {
            return context.ServiceGallery.Where(s => s.IsActive && !s.IsDelete).Select(s => new ServiceGalleryDTO
            {
                OrderId = s.OrderId,
                Title = s.Title,
                SubTitle = s.SubTitle,
                Link = s.Link,
                Image = s.Image
            });
        }
    }
}
