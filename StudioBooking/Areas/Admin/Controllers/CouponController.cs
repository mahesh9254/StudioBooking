using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudioBooking.Areas.Admin.Models;
using StudioBooking.Data;
using StudioBooking.Data.Models;
using StudioBooking.DTO;
using StudioBooking.Infrastructure;
using static StudioBooking.Infrastructure.Enums;

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
        public async Task<IActionResult> GetCoupons()
        {
            var coupons = await _context.Coupons.Where(c =>  !c.IsDelete ).Select(c => new CouponViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                Discount = c.Discount,                
                IsActive = c.IsActive,
                
            }).ToListAsync();

            return Ok(new { data = coupons });
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
                if (coupon != null) 
                {
                    model.Id = coupon.Id;
                    model.Name = coupon.Name;
                    model.Code = coupon.Code;                    
                    model.Discount = coupon.Discount;
                    model.IsActive = coupon.IsActive;

                }
                
                return PartialView("_CouponModal", model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCoupon([FromBody] CouponDTO couponDTO)
        {
            try
            {
                var coupon = new Coupon
                {
                    Id = couponDTO.Id,
                    Name = couponDTO.Name,
                    Code = couponDTO.Code,                    
                    Discount = couponDTO.Discount,
                    IsActive = true,
                    IsDelete = false,
                    CreatedBy = GetUserId(),
                    CreatedDate = Defaults.GetDateTime(),
                    ModifiedBy = GetUserId(),
                    ModifiedDate = Defaults.GetDateTime()
                };

                var couponAll = await _context.Coupons.ToListAsync();
                foreach (var item in couponAll)
                {
                    item.IsActive = false;
                }
                await _context.Coupons.AddAsync(coupon);                
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Ok(new { result = false, error = ex.Message });
            }
            return Ok(new { result = true });
        }

        [HttpPost]
        public async Task<IActionResult> EditCoupon([FromBody] CouponDTO couponDTO)
        {
            try
            {
                var coupon = await _context.Coupons.FirstOrDefaultAsync(b => b.Id == couponDTO.Id) ?? throw new Exception("Invalid coupon id");
                coupon.Discount = couponDTO.Discount;
                coupon.ModifiedBy = GetUserId();
                coupon.ModifiedDate = DateTime.Now;               
                
                await _context.SaveChangesAsync();
               
            }
            catch (Exception ex)
            {
                _logger.LogError("EditCoupon => " + ex.Message, ex);
                return Ok(new { data = couponDTO, result = false, error = ex.Message });
            }
            return Ok(new { data = couponDTO, result = true });
        }

    }
}
