using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudioBooking.Data;
using StudioBooking.Data.Models;
using StudioBooking.DTO;
using StudioBooking.Infrastructure;

namespace StudioBooking.Controllers
{
    [ServiceFilter(typeof(ControllerActionFilter))]
    public class BaseController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public BaseController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected override void Dispose(bool disposing)
        {
            _userManager.Dispose();
            base.Dispose(disposing);
        }

        public string GetUserId()
        {
            return _userManager.GetUserId(User) ?? throw new ArgumentException();
        }
    }
}
