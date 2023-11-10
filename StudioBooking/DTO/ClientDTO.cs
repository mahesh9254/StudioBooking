using Microsoft.EntityFrameworkCore;
using StudioBooking.Data;
using StudioBooking.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace StudioBooking.DTO
{
    public class ClientDTO
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string? Name { get; set; }
        [MaxLength(500)]
        public string? Summary { get; set; }
        [MaxLength(100)]
        public string? Image { get; set; }
        public IFormFile? ImageName { get; set; }
        public bool EnablePulse { get; set; }
        public int? Order { get; set; }
        public bool IsActive { get; set; }
        [MaxLength(100)]
        public string? CreatedBy { get; set; }
        public string? CreatedDate { get; set; }

        internal static async Task<List<ClientDTO>> GetClients(ApplicationDbContext context, bool getAll = false)
        {
            var clients = await context.Clients.Where(c => !c.IsDelete).Select(c => new ClientDTO
            {
                Id = c.Id,
                Name = c.Name,
                Summary = c.Summary,
                Image = c.Image,
                EnablePulse = c.EnablePulse,
                Order = c.Order ?? c.Id,
                IsActive = c.IsActive,
                CreatedBy = c.CreatedBy,
                CreatedDate = c.CreatedDate.ToShortDateString()
            }).ToListAsync();
            return getAll ? clients : clients.Where(c => c.IsActive).ToList();
        }

        internal static async Task<ClientDTO> GetClient(ApplicationDbContext context, int id)
        {
            var client = await context.Clients.FirstOrDefaultAsync(c => c.Id == id && !c.IsDelete) ?? new Client();
            return new ClientDTO
            {
                Id = client.Id,
                Name = client.Name,
                Summary = client.Summary,
                Image = client.Image,
                EnablePulse = client.EnablePulse,
                Order = client.Order,
                IsActive = client.IsActive,
                CreatedBy = client.CreatedBy,
                CreatedDate = client.CreatedDate.ToShortDateString()
            };
        }
    }
}
