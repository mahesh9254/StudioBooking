using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudioBooking.Data;
using StudioBooking.Data.Models;
using StudioBooking.Infrastructure;

namespace StudioBooking.Areas.User.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(ControllerActionFilter))]
    public class BaseController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly RoleManager<IdentityRole> _roleManager;
        public BaseController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            //_roleManager = roleManager;
            //_context = applicationDbContext;
        }

        protected override void Dispose(bool disposing)
        {
            _userManager.Dispose();
            //_roleManager.Dispose();
            //_context.Dispose();
            base.Dispose(disposing);
        }

        public string GetUserId()
        {
            return _userManager.GetUserId(User) ?? throw new ArgumentException();
        }


    }
}
