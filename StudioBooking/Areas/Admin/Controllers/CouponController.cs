using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudioBooking.Areas.Admin.Models;
using StudioBooking.Data;
using StudioBooking.Data.Models;

namespace StudioBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CouponController :  BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CouponController> _logger;
        public CouponController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ILogger<CouponController> logger) : base(userManager)
        {
            _context = context;           
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> CreateCouponModal(long? id)
        {
            if (id == null)
            {
                var model = new CouponViewModel();
                
                return PartialView("_CouponModal", model);
            }
            else
            {
                
                var coupon = await _context.Coupons.FirstOrDefaultAsync(s => s.Id == id);
                var model = new CouponViewModel();
                if (coupon == null) 
                {
                    model.Id = coupon.Id;
                    model.Name = coupon.Name;
                    model.Code = coupon.Code;
                    model.DiscountType = coupon.DiscountType;
                    model.Discount = coupon.Discount;
                    model.IsActive = coupon.IsActive;

                }
                
                return PartialView("_CouponModal", model);
            }
        }
    }
}
