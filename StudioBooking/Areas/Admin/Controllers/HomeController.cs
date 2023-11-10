using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudioBooking.Areas.Admin.Models;
using StudioBooking.Data;
using StudioBooking.Data.Models;
using StudioBooking.Infrastructure;
using System.Linq;
using static StudioBooking.Infrastructure.Enums;

namespace StudioBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public HomeController(UserManager<ApplicationUser> userManager,ApplicationDbContext applicationDbContext): base(userManager)
        {
            _context = applicationDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings.Where(b => b.BookingDate > Defaults.GetDateTime() && (b.BookingStatus != (int)BookingStatus.Cancelled || b.BookingStatus != (int)BookingStatus.Failed)).Select(b => new { b.BookingDate, b.StartTime, b.EndTime, b.BookingStatus, b.PaymentStatus }).ToListAsync();
            var dashaboardViewModel = new DashboardViewModel
            {
                AdvanceBookings = bookings.Where(b => b.BookingStatus == (int)BookingStatus.Booked && b.PaymentStatus == (int)PaymentStatus.Advance).Count(),
                ApprovalPending = bookings.Where(b => b.BookingStatus == (int)BookingStatus.WaitingForApproval).Count(),
                PendingBookings = bookings.Where(b => b.BookingStatus == (int)BookingStatus.Pending && b.PaymentStatus == (int)PaymentStatus.Pending).Count(),
                TotalBookedHours = bookings.Where(b => b.BookingStatus == (int)BookingStatus.Booked || b.BookingStatus == (int)BookingStatus.ReScheduled).Select(s => (TimeOnly.Parse(s.EndTime) - TimeOnly.Parse(s.StartTime)).TotalHours).Sum(),
            };
            return View(dashaboardViewModel);
        }
    }
}
