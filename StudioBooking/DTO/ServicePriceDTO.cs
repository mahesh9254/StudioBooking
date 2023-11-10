using Microsoft.EntityFrameworkCore;
using StudioBooking.Data;
using StudioBooking.Data.Models;
using static StudioBooking.Infrastructure.Enums;

namespace StudioBooking.DTO
{
    public class ServicePriceDTO
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryTitle { get; set; }
        public string? CategoryDesciption { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public int ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public ServiceType ServiceType { get; set; }
        public bool EnableTwoStepBooking { get; set; }
        public float Price { get; set; }
        public int MinHours { get; set; }
        public string? EventColorId { get; set; }
        public string? CalenderName { get; set; }
        public string? Description { get; set; }
        public string? AdditionalInformation { get; set; }
        public string? Image { get; set; }
        public bool DisableBooking { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedDate { get; set; }
        public static async Task<ServicePriceDTO> GetServicePrice(ApplicationDbContext dbContext, int id)
        {
            var servicePriceInDb = await dbContext.ServicePrices.Include(c => c.Category).Include(s => s.Service).FirstOrDefaultAsync(c => c.Id == id && c.IsActive && !c.IsDelete);
            return new ServicePriceDTO
            {
                Id = id,
                CategoryId = servicePriceInDb.CategoryId,
                CategoryName = servicePriceInDb.Category.Name,
                CategoryDesciption = servicePriceInDb.Category.Description,
                StartTime = servicePriceInDb.Category.StartTime,
                EndTime = servicePriceInDb.Category.EndTime,
                Image = servicePriceInDb.Category.Image,
                ServiceId = servicePriceInDb.ServiceId,
                ServiceName = servicePriceInDb.Service.Name,
                ServiceType = (ServiceType)servicePriceInDb.Service.ServiceType,
                EnableTwoStepBooking = servicePriceInDb.Service.EnableTwoStepBooking,
                Price = servicePriceInDb.Price,
                MinHours = servicePriceInDb.MinHours,
                EventColorId = servicePriceInDb.EventColorId,
                CalenderName = servicePriceInDb.CalenderName,
                Description = servicePriceInDb.Description,
                AdditionalInformation = servicePriceInDb.AdditionalInformation,
                DisableBooking = servicePriceInDb.DisableBooking ?? false,
                CreatedBy = servicePriceInDb.CreatedBy,
                CreatedDate = servicePriceInDb.CreatedDate.ToShortDateString()
            };
        }

        public static async Task<List<ServicePriceDTO>> GetServicePrices(ApplicationDbContext dbContext)
        {
            return MapServicePrices(await dbContext.ServicePrices.Include(c => c.Category).Include(s => s.Service).Where(c => c.IsActive && !c.IsDelete && !(c.DisableBooking ?? false)).ToListAsync());
        }

        public static async Task<List<ServicePriceDTO>> GetServicePricesByCategory(ApplicationDbContext dbContext, int categoryId)
        {
            return MapServicePrices(await dbContext.ServicePrices.Include(c => c.Category).Include(s => s.Service).Where(c => c.CategoryId == categoryId && c.IsActive && !c.IsDelete).ToListAsync());
        }

        public static async Task<List<ServicePriceDTO>> GetServicePricesByService(ApplicationDbContext dbContext, int serviceId)
        {
            return MapServicePrices(await dbContext.ServicePrices.Include(c => c.Category).Include(s => s.Service).Where(c => c.ServiceId == serviceId && c.IsActive && !c.IsDelete).ToListAsync());
        }

        private static List<ServicePriceDTO> MapServicePrices(List<ServicePrice> servicePrices)
        {
            return servicePrices.Select(s => new ServicePriceDTO
            {
                Id = s.Id,
                CategoryId = s.CategoryId,
                CategoryName = s.Category.Name,
                CategoryTitle = s.Category.Title,
                ServiceId = s.ServiceId,
                ServiceName = s.Service.Name,
                ServiceType = (ServiceType)s.Service.ServiceType,
                EnableTwoStepBooking = s.Service.EnableTwoStepBooking,
                Price = s.Price,
                MinHours = s.MinHours,
                EventColorId = s.EventColorId,
                CalenderName = s.CalenderName,
                StartTime = s.Category.StartTime,
                EndTime = s.Category.EndTime,
                Description = s.Description,
                CategoryDesciption = s.Category.Description,
                AdditionalInformation = s.AdditionalInformation,
                DisableBooking = s.DisableBooking ?? false,
                CreatedBy = s.CreatedBy,
                CreatedDate = s.CreatedDate.ToShortDateString()
            }).OrderByDescending(s => s.ServiceType).ToList();
        }

        public static ServicePriceDTO GetServicePrice(ServicePrice servicePrice)
        {
            return new ServicePriceDTO
            {
                Id = servicePrice.Id,
                CategoryId = servicePrice.Category.Id,
                CategoryName = servicePrice.Category.Name,
                CategoryDesciption = servicePrice.Category.Description,
                Image = servicePrice.Category.Image,
                ServiceId = servicePrice.ServiceId,
                ServiceName = servicePrice.Service.Name,
                ServiceType = (ServiceType)servicePrice.Service.ServiceType,
                EnableTwoStepBooking = servicePrice.Service.EnableTwoStepBooking,
                StartTime = servicePrice.Category.StartTime,
                EndTime = servicePrice.Category.EndTime,
                Price = servicePrice.Price,
                MinHours = servicePrice.MinHours,
                EventColorId = servicePrice.EventColorId,
                CalenderName = servicePrice.CalenderName,
                Description = servicePrice.Description,
                AdditionalInformation = servicePrice.AdditionalInformation,
                DisableBooking = servicePrice.DisableBooking ?? false,
                CreatedBy = servicePrice.CreatedBy,
                CreatedDate = servicePrice.CreatedDate.ToShortDateString()
            };
        }
    }
}
