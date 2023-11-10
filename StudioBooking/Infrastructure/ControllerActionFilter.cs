using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using StudioBooking.Data;
using StudioBooking.Data.Models;
using StudioBooking.DTO;
using StudioBooking.ViewModels;

namespace StudioBooking.Infrastructure
{
    public class ControllerActionFilter : IAsyncActionFilter
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ControllerActionFilter(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var area = string.Empty;
            var controller = context.Controller as Controller;
            if (controller == null) await next();

            area = (string?)(controller.RouteData.Values["area"] ?? null);
            var layoutViewModel = new LayoutViewModel();
            //if (context.HttpContext.User.Identity == null)
            //{
            //    context.HttpContext.Session.Clear();
            //    await context.HttpContext.SignOutAsync();
            //}
            //else

            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var userName = context.HttpContext.User.Identity.Name;
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
                var roles = await _userManager.GetRolesAsync(user);
                //var superUserRoleId = _context.Roles.Where(r => r.Name == UserRole.SuperAdmin).Select(a => a.Id).FirstOrDefault();
                layoutViewModel.ProfilePic = !string.IsNullOrEmpty(user.ProfileImageUrl) ? AppConfig.ProfileImageUrl + user.ProfileImageUrl : Defaults.ProfileImage;
                layoutViewModel.UserName = user?.UserName;
                layoutViewModel.FirstName = user.FirstName;
                layoutViewModel.LastName = user.LastName;
                layoutViewModel.Mobile = user.PhoneNumber;
                layoutViewModel.Role = roles.FirstOrDefault();
            }
            if (string.IsNullOrEmpty(area) || (area ?? "") == "User")
            {
                layoutViewModel.WebsiteSetting = await WebsiteSettingDTO.GetWebsiteSettingAsync(_context);
                layoutViewModel.Categories = await _context.Categories.Where(c => c.IsActive).Select(s => new CategoryDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Image = AppConfig.CategoryImageUrl + s.Image,
                }).ToListAsync();
            }
            controller.ViewData[nameof(LayoutViewModel)] = layoutViewModel;

            var result = await next();
        }
    }
}
