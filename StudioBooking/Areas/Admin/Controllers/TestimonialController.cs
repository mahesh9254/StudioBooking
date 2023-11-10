using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudioBooking.Areas.Admin.Models;
using StudioBooking.Data;
using StudioBooking.Data.Models;
using StudioBooking.DTO;
using StudioBooking.Infrastructure;

namespace StudioBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class TestimonialController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _filePath;
        public TestimonialController(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext, IWebHostEnvironment webHostEnvironment) : base(userManager)
        {
            _context = applicationDbContext;
            _webHostEnvironment = webHostEnvironment;
            _filePath = _webHostEnvironment.WebRootPath + AppConfig.ClientImageUrl.Replace("/", "\\");
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View(new TestimonialViewModel());
        }

        public async Task<IActionResult> Edit(int id)
        {
            var testimonialViewModel = new TestimonialViewModel
            {
                Testimonial = await TestimonialDTO.GetTestimonial(_context, id)
            };
            return View("Create", testimonialViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SaveTestimonial(TestimonialViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var testimonialInDb = await _context.Testimonials.FirstOrDefaultAsync(c => c.Id == model.Testimonial.Id && !c.IsDelete);
                    if (testimonialInDb == null)
                    {

                        var testimonial = new Testimonial
                        {
                            Name = model.Testimonial.Name,
                            Professions = model.Testimonial.Professions,
                            Review = model.Testimonial.Review,
                            Rating = model.Testimonial.Rating,
                            CreatedBy = GetUserId(),
                            CreatedDate = Defaults.GetDateTime(),
                            ModifiedBy = GetUserId(),
                            ModifiedDate = Defaults.GetDateTime(),
                            IsActive = true,
                            Image = model.Testimonial.ImageName != null ? new Common().UploadedFile(model.Testimonial.ImageName, _filePath) : string.Empty,
                        };
                        await _context.Testimonials.AddAsync(testimonial);
                    }
                    else
                    {
                        testimonialInDb.Name = model.Testimonial.Name;
                        testimonialInDb.Review = model.Testimonial.Review;
                        testimonialInDb.Rating = model.Testimonial.Rating;
                        testimonialInDb.Professions = model.Testimonial.Professions;
                        testimonialInDb.ModifiedBy = GetUserId();
                        testimonialInDb.ModifiedDate = Defaults.GetDateTime();
                        testimonialInDb.Image = model.Testimonial.ImageName != null ? new Common().UploadedFile(model.Testimonial.ImageName, _filePath) : testimonialInDb.Image;
                    }
                    await _context.SaveChangesAsync();
                    model.Result = true;
                }
                catch (Exception ex)
                {
                    model.ErrorMessage = ex.Message;
                    model.Result = false;
                }
            }
            return View("Create", model);
        }


        [HttpGet]
        public async Task<IActionResult> GetTestimonials()
        {
            var testimonials = await TestimonialDTO.GetTestimonials(_context, true);
            return Ok(new { data = testimonials });
        }
    }
}
