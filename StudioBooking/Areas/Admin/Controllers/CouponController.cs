using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudioBooking.Data.Models;

namespace StudioBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CouponController :  BaseController
    {
        public CouponController(UserManager<ApplicationUser> userManager) : base(userManager)
        {
            
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
