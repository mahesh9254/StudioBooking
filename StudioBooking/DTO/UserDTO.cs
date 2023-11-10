using Microsoft.EntityFrameworkCore;
using StudioBooking.Data;
using StudioBooking.Data.Models;
using StudioBooking.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace StudioBooking.DTO
{
    public class UserDTO
    {
        public string? UserId { get; set; }
        public string? CustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [StringLength(10)]
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? Name { get; internal set; }
        public string? Role { get; set; }
        public string? ProfileImageUrl { get; internal set; }
        public IFormFile? ImageName { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
        public string? CreatedDate { get; set; }
        public List<CustomerDTO>? Customers { get; set; }       

        public static async Task<UserDTO> GetUser(ApplicationDbContext context, string userId)
        {
            var user = await context.Users.Include(u => u.Customers).FirstOrDefaultAsync(u => u.Id == userId);
            return new UserDTO
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Mobile = user.PhoneNumber,
                ProfileImageUrl = user.ProfileImageUrl,
                CreatedDate = user.CreatedDate.ToShortDateString(),
                Customers = user.Customers.Select(u => new CustomerDTO
                {
                    Id = u.Id,
                    UserId = u.UserId,
                    Name = u.Name,
                    CompanyName = u.CompanyName,
                    //GstNumber = u.GstNumber,
                    //AddressLine1 = u.AddressLine1,
                    //AddressLine2 = u.AddressLine2,
                    //Landmark = u.Landmark,
                    //City = u.City,
                    //State = u.State,
                    //PinCode = u.PinCode
                }).ToList()
            };
        }
    }
}
