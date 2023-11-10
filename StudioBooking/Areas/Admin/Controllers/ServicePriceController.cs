using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    [Authorize]
    public class ServicePriceController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public ServicePriceController(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext) : base(userManager)
        {
            _context = applicationDbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create()
        {
            var servicePriceViewModel = new ServicePriceViewModel
            {
                Categories = new SelectList(await _context.Categories.Where(c => c.IsActive && !c.IsDelete).Select(s => new { s.Id, s.Name }).ToListAsync(), "Id", "Name", null),
                Services = new SelectList(await _context.Services.Where(c => c.IsActive && !c.IsDelete).Select(s => new { s.Id, s.Name }).ToListAsync(), "Id", "Name", null),
                EventColors = new SelectList(EventColor.GetEventColors().Select(s => new { s.Id, s.Background }).ToList(), "Id", "Background", null),
            };
            return View(servicePriceViewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var servicePriceInDb = await ServicePriceDTO.GetServicePrice(_context, id);
            var servicePriceViewModel = new ServicePriceViewModel
            {
                ServicePrice = servicePriceInDb,
                Categories = new SelectList(await _context.Categories.Where(c => c.IsActive && !c.IsDelete).Select(s => new { s.Id, s.Name }).ToListAsync(), "Id", "Name", servicePriceInDb.CategoryId),
                Services = new SelectList(await _context.Services.Where(c => c.IsActive && !c.IsDelete).Select(s => new { s.Id, s.Name }).ToListAsync(), "Id", "Name", servicePriceInDb.ServiceId),
                EventColors = new SelectList(EventColor.GetEventColors().Select(s => new { s.Id, s.Background }).ToList(), "Id", "Background", servicePriceInDb.Id),
            };
            return View("Create", servicePriceViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetServicePriceList()
        {
            var servicePrices = await _context.ServicePrices.Include(c => c.Category).Include(s => s.Service).Where(c => c.IsActive && !c.IsDelete).Select(s => new ServicePriceDTO
            {
                Id = s.Id,
                CategoryId = s.CategoryId,
                CategoryName = s.Category.Name,
                ServiceId = s.ServiceId,
                ServiceName = s.Service.Name,
                ServiceType = (ServiceType)s.Service.ServiceType,
                Price = s.Price,
                MinHours = s.MinHours,
                EventColorId = EventColor.GetEventColor(s.EventColorId ?? "2").Background,
                CalenderName = s.CalenderName,
                Description = s.Description,
                AdditionalInformation = s.AdditionalInformation,
                DisableBooking = s.DisableBooking ?? false,
                CreatedBy = s.CreatedBy,
                CreatedDate = s.CreatedDate.ToShortDateString()
            }).ToListAsync();

            return Ok(new { data = servicePrices });
        }

        [HttpGet]
        public async Task<IActionResult> GetServices(long id)
        {
            var servicePrices = await _context.ServicePrices.Include(s => s.Category).Include(s => s.Service).Where(c => c.CategoryId == id && c.IsActive && !c.IsDelete).Select(s => new ServicePriceDTO
            {
                Id = s.Id,
                CategoryId = s.CategoryId,
                ServiceId = s.ServiceId,
                ServiceName = s.Service.Name,
                ServiceType = (ServiceType)s.Service.ServiceType,
                Price = s.Price,
                MinHours = s.MinHours,
                Description = s.Description,
                StartTime = s.Category.StartTime,
                EndTime = s.Category.EndTime
            }).ToListAsync();

            return Ok(new { data = servicePrices });
        }

        [HttpPost]
        public async Task<IActionResult> SaveServicePrice(ServicePriceViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var servicePrinceInDb = await _context.ServicePrices.FirstOrDefaultAsync(c => c.Id == model.ServicePrice.Id);
                    if (servicePrinceInDb == null)
                    {
                        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == model.ServicePrice.CategoryId);
                        var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == model.ServicePrice.ServiceId);
                        var servicePrice = new ServicePrice
                        {
                            //CategoryId = model.ServicePrice.CategoryId,
                            //ServiceId = model.ServicePrice.ServiceId,
                            Category = category ?? new Category(),
                            Service = service ?? new Service(),
                            Price = model.ServicePrice.Price,
                            MinHours = model.ServicePrice.MinHours,
                            Description = model.ServicePrice.Description,
                            AdditionalInformation = model.ServicePrice.AdditionalInformation,
                            //DisableBooking = model.ServicePrice.DisableBooking,
                            EventColorId = model.ServicePrice.EventColorId,
                            CalenderName = model.ServicePrice.CalenderName,
                            CreatedBy = GetUserId(),
                            CreatedDate = Defaults.GetDateTime(),
                            ModifiedBy = GetUserId(),
                            ModifiedDate = Defaults.GetDateTime(),
                            IsActive = true
                        };
                        await _context.ServicePrices.AddAsync(servicePrice);
                    }
                    else
                    {
                        servicePrinceInDb.CategoryId = model.ServicePrice.CategoryId;
                        servicePrinceInDb.ServiceId = model.ServicePrice.ServiceId;
                        servicePrinceInDb.Price = model.ServicePrice.Price;
                        servicePrinceInDb.MinHours = model.ServicePrice.MinHours;
                        servicePrinceInDb.EventColorId = model.ServicePrice.EventColorId;
                        servicePrinceInDb.CalenderName = model.ServicePrice.CalenderName;
                        servicePrinceInDb.Description = model.ServicePrice.Description;
                        servicePrinceInDb.AdditionalInformation = model.ServicePrice.AdditionalInformation;
                        //servicePrinceInDb.DisableBooking = model.ServicePrice.DisableBooking;
                        servicePrinceInDb.ModifiedBy = GetUserId();
                        servicePrinceInDb.ModifiedDate = Defaults.GetDateTime();
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
        public async Task<IActionResult> ToggleBookingStatus(int id, bool status)
        {
            var result = false;
            var errMsg = string.Empty;
            try
            {
                var servicePrice = await _context.ServicePrices.FirstOrDefaultAsync(c => c.Id == id) ?? throw new InvalidOperationException("Invalid Service Price ID");
                servicePrice.DisableBooking = !status;
                servicePrice.ModifiedBy = GetUserId();
                servicePrice.ModifiedDate = Defaults.GetDateTime();
                await _context.SaveChangesAsync();
                result = true;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return Ok(new { result, errMsg });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var result = false;
            var errMsg = string.Empty;
            try
            {
                var service = await _context.ServicePrices.FindAsync(id);
                if (service == null)
                    throw new InvalidOperationException("Invalid Service Price Id");
                service.IsActive = false;
                service.IsDelete = true;
                service.ModifiedBy = GetUserId();
                service.ModifiedDate = Defaults.GetDateTime();
                await _context.SaveChangesAsync();
                result = true;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return Ok(new { result, errMsg });
        }
    }
}
