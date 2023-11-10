using Microsoft.EntityFrameworkCore;
using StudioBooking.Data;
using StudioBooking.Data.Models;
using StudioBooking.Infrastructure;
using System.ComponentModel.DataAnnotations;
using static StudioBooking.Infrastructure.Enums;

namespace StudioBooking.DTO
{
    public class ServiceDTO
    {
        public int Id { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }
        public ServiceType ServiceType { get; set; }
        public bool EnableTwoStepBooking { get; set; }
        public string? ServiceTypeName
        {
            get
            {
                return Enum.GetName(typeof(Enums.ServiceType), ServiceType);
            }
            set
            {
                _serviceTypeName = value;
            }
        }
        public string? CreatedBy { get; set; }
        public string? CreatedDate { get; set; }
        private string? _serviceTypeName;

        public static async Task<ServiceDTO> GetService(ApplicationDbContext dbContext, int id)
        {
            var serviceInDb = await dbContext.Services.FirstOrDefaultAsync(c => c.Id == id && !c.IsDelete) ?? new Service();
            return new ServiceDTO
            {
                Id = id,
                Name = serviceInDb.Name,
                EnableTwoStepBooking = serviceInDb.EnableTwoStepBooking,
                ServiceType = (ServiceType)serviceInDb.ServiceType
            };
        }

        public static async Task<List<ServiceDTO>> GetServices(ApplicationDbContext dbContext)
        {
            return await dbContext.Services.Where(c => c.IsActive && !c.IsDelete).Select(s => new ServiceDTO
            {
                Id = s.Id,
                Name = s.Name,
                EnableTwoStepBooking = s.EnableTwoStepBooking,
                ServiceType = (ServiceType)s.ServiceType
            }).ToListAsync();
        }
    }
}
