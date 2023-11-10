using Microsoft.EntityFrameworkCore;
using StudioBooking.Data;
using StudioBooking.Data.Models;
using StudioBooking.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace StudioBooking.DTO
{
    public class TestimonialDTO
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string? Name { get; set; }
        [MaxLength(200)]
        public string? Professions { get; set; }
        public List<string> ProfessionNames { get; set; }
        [MaxLength(500)]
        public string? Review { get; set; }
        [MaxLength(100)]
        public string? Image { get; set; }
        [MaxLength(50)]
        public string? Rating { get; set; }
        public IFormFile? ImageName { get; set; }
        public bool IsActive { get; set; }
        [MaxLength(100)]
        public string? CreatedBy { get; set; }
        public string? CreatedDate { get; set; }
        public TestimonialDTO()
        {
            ProfessionNames = new List<string>();
        }

        internal static async Task<TestimonialDTO> GetTestimonial(ApplicationDbContext context, int id)
        {
            var testimonial = await context.Testimonials.FirstOrDefaultAsync(c => c.Id == id && !c.IsDelete) ?? new Testimonial();
            return new TestimonialDTO
            {
                Id = testimonial.Id,
                Name = testimonial.Name,
                Review = testimonial.Review,
                Rating = testimonial.Rating,
                Professions = testimonial.Professions,
                ProfessionNames = string.IsNullOrEmpty(testimonial.Professions) ? new List<string>() : GetProfessionsNames(testimonial.Professions),
                Image = testimonial.Image,
                IsActive = testimonial.IsActive,
                CreatedBy = testimonial.CreatedBy,
                CreatedDate = testimonial.CreatedDate.ToShortDateString()
            };
        }

        internal static async Task<List<TestimonialDTO>> GetTestimonials(ApplicationDbContext context, bool getAll = false)
        {
            var testimonials = await context.Testimonials.Where(c => !c.IsDelete).Select(c => new TestimonialDTO
            {
                Id = c.Id,
                Name = c.Name,
                Review = c.Review,
                Rating = c.Rating,
                Image = c.Image,
                Professions = c.Professions,
                ProfessionNames = string.IsNullOrEmpty(c.Professions) ? new List<string>() : GetProfessionsNames(c.Professions),
                IsActive = c.IsActive,
                CreatedBy = c.CreatedBy,
                CreatedDate = c.CreatedDate.ToShortDateString()
            }).ToListAsync();
            return getAll ? testimonials : testimonials.Where(c => c.IsActive).ToList();
        }

        private static List<string> GetProfessionsNames(string values) => values.Split(",").Select(s => Enum.GetName(typeof(Enums.Professions), Convert.ToInt16(s))).ToList();
    }
}
