using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudioBooking.Areas.User.Models;
using StudioBooking.Data;
using StudioBooking.Data.Models;
using StudioBooking.DTO;
using StudioBooking.Infrastructure;

namespace StudioBooking.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public HomeController(UserManager<ApplicationUser> userManager, ApplicationDbContext context) : base(userManager)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardViewModel = new DashboardViewModel
            {
                User = await UserDTO.GetUser(_context, GetUserId()),
                Bookings = await BookingDTO.GetUserBookings(_context, GetUserId(), Defaults.GetDateTime()),
                Transactions = await TransactionDTO.GetUserTransactions(_context, GetUserId(), true)
            };
            return View(dashboardViewModel);
        }
    }
}
