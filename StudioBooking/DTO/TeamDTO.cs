using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using StudioBooking.Data;
using StudioBooking.Data.Models;
using StudioBooking.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace StudioBooking.DTO
{
    public class TeamDTO
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string? Name { get; set; }
        [MaxLength(200)]
        public string? Professions { get; set; }
        public List<string> ProfessionNames { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        [MaxLength(100)]
        public string? Image { get; set; }
        public IFormFile? ImageName { get; set; }
        public bool IsActive { get; set; }
        [MaxLength(100)]
        public string? CreatedBy { get; set; }
        public string? CreatedDate { get; set; }
        public TeamDTO()
        {
            ProfessionNames = new List<string>();
        }

        internal static async Task<List<TeamDTO>> GetTeams(ApplicationDbContext context, bool getAll = false)
        {
            var teams = await context.Teams.Where(c => !c.IsDelete).Select(c => new TeamDTO
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Image = c.Image,
                Professions = c.Professions,
                ProfessionNames = string.IsNullOrEmpty(c.Professions) ? new List<string>() : GetProfessionsNames(c.Professions),
                IsActive = c.IsActive,
                CreatedBy = c.CreatedBy,
                CreatedDate = c.CreatedDate.ToShortDateString()
            }).ToListAsync();
            return getAll ? teams : teams.Where(c => c.IsActive).ToList();
        }

        internal static async Task<TeamDTO> GetTeam(ApplicationDbContext context, int id)
        {
            var team = await context.Teams.FirstOrDefaultAsync(c => c.Id == id && !c.IsDelete) ?? new Team();
            return new TeamDTO
            {
                Id = team.Id,
                Name = team.Name,
                Description = team.Description,
                Professions = team.Professions,
                ProfessionNames = string.IsNullOrEmpty(team.Professions) ? new List<string>() : GetProfessionsNames(team.Professions),
                Image = team.Image,
                IsActive = team.IsActive,
                CreatedBy = team.CreatedBy,
                CreatedDate = team.CreatedDate.ToShortDateString()
            };
        }

        private static List<string> GetProfessionsNames(string values) => values.Split(",").Select(s => Enum.GetName(typeof(Enums.Professions), Convert.ToInt16(s))).ToList();
    }
}
