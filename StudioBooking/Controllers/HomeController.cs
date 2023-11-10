using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudioBooking.Data;
using StudioBooking.Data.Models;
using StudioBooking.DTO;
using StudioBooking.Infrastructure;
using StudioBooking.Models;
using StudioBooking.ViewModels;
using System.Diagnostics;

namespace StudioBooking.Controllers
{

    public class HomeController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly IGoogleCalendar _googleCalendar;
        private readonly IEmailNotification _emailNotification;

        public HomeController(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, ILogger<HomeController> logger, IGoogleCalendar googleCalendar, IEmailNotification emailNotification) : base(userManager)
        {
            _logger = logger;
            _googleCalendar = googleCalendar;
            _context = applicationDbContext;
            _emailNotification = emailNotification;
        }

        public async Task<IActionResult> Index()
        {
            //Register
            //var user = await _context.Users.FirstOrDefaultAsync(s=> s.UserName == "anup1421@gmail.com");
            //var websiteSetting = await WebsiteSettingDTO.GetWebsiteSettingAsync(_context);
            //await _emailNotification.SendUserRegisterNotification(user.Email,user, websiteSetting);

            //ConfirmBooking
            //var booking = await BookingDTO.GetBookingDetailById(_context, 91);
            //var serviceDetail = await ServicePriceDTO.GetServicePrice(_context, booking.ServicePriceId);
            //booking.ServiceName = serviceDetail.ServiceName;
            //booking.CategoryName = serviceDetail.CategoryName;
            //booking.ServiceTitle = Defaults.GetServiceTitle(serviceDetail.ServiceName);
            //var user = await _context.Users.FirstOrDefaultAsync(s => s.UserName == "anup1421@gmail.com");
            //var websiteSetting = await WebsiteSettingDTO.GetWebsiteSettingAsync(_context);
            //await _emailNotification.SendBookingNotification(booking, user, websiteSetting);

            ////Reschedule
            //var booking = await BookingDTO.GetBookingDetailById(_context, 12);
            //var scheduleRequest = await _context.ScheduleRequests.FirstOrDefaultAsync(s => s.BookingId == 12);
            //var serviceDetail = await ServicePriceDTO.GetServicePrice(_context, booking.ServicePriceId);
            //booking.ServiceName = serviceDetail.ServiceName;
            //booking.CategoryName = serviceDetail.CategoryName;
            //booking.ServiceTitle = Defaults.GetServiceTitle(serviceDetail.ServiceName);            
            //var user = await _context.Users.FirstOrDefaultAsync(s => s.UserName == "anup1421@gmail.com");
            //var websiteSetting = await WebsiteSettingDTO.GetWebsiteSettingAsync(_context);
            //await _emailNotification.SendReScheduleCancellationNotification(scheduleRequest, booking, user, websiteSetting);

            var studioViewModel = new StudioViewModel
            {
                Categories = await _context.Categories.Where(c => c.IsActive).Select(s => new CategoryDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    Title = s.Title,
                    Description = s.Description,
                    Image = s.Image,
                }).ToListAsync(),
                Services = await ServiceDTO.GetServices(_context),
                Clients = await ClientDTO.GetClients(_context),
                Testimonials = await TestimonialDTO.GetTestimonials(_context),
                Teams = await TeamDTO.GetTeams(_context)
                //ServiceGalleries = ServiceGalleryDTO.GetServiceGalleries(_context)
            };
            return View(studioViewModel);
        }

        public async Task<IActionResult> Studio(int id)
        {
            var studioViewModel = new StudioViewModel
            {
                Category = await CategoryDTO.GetCategory(_context, id),
                CategoryDetails = await CategoryDetailDTO.GetCategoryDetails(_context, id),
                ServicePrices = await ServicePriceDTO.GetServicePricesByCategory(_context, id),
            };
            return View(studioViewModel);
        }

        public async Task<IActionResult> About()
        {
            var studioViewModel = new StudioViewModel
            {
                CategoryDetails = await CategoryDetailDTO.GetCategoryDetails(_context),
                Teams = await TeamDTO.GetTeams(_context)
            };
            return View(studioViewModel);
        }

        public async Task<IActionResult> OurStudios()
        {
            var studioViewModel = new StudioViewModel
            {
                CategoryDetails = await CategoryDetailDTO.GetCategoryDetails(_context),
                Teams = await TeamDTO.GetTeams(_context)
            };
            return View(studioViewModel);
        }

        public async Task<IActionResult> Testimonials()
        {
            var studioViewModel = new StudioViewModel
            {
                Testimonials = await TestimonialDTO.GetTestimonials(_context),
                Clients = await ClientDTO.GetClients(_context)
            };
            return View(studioViewModel);
        }

        public async Task<IActionResult> Rates()
        {
            var studioViewModel = new StudioViewModel
            {
                Categories = await _context.Categories.Where(c => c.IsActive).Select(s => new CategoryDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    Title = s.Title,
                    Description = s.Description,
                    Image = s.Image,
                }).ToListAsync(),
                ServicePrices = await ServicePriceDTO.GetServicePrices(_context)                
            };
            return View(studioViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult TermsAndConditions()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}