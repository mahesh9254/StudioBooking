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
    public class ServiceController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public ServiceController(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext) : base(userManager)
        {
            _context = applicationDbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View(new ServiceViewModel());
        }

        public async Task<IActionResult> Edit(int id)
        {
            var categoryViewModel = new ServiceViewModel
            {
                Service = await ServiceDTO.GetService(_context, id),
            };
            return View("Create", categoryViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetServiceList()
        {
            var services = await _context.Services.Where(c => c.IsActive && !c.IsDelete).Select(s => new ServiceDTO
            {
                Id = s.Id,
                Name = s.Name,
                EnableTwoStepBooking = s.EnableTwoStepBooking,
                ServiceType = (Enums.ServiceType)s.ServiceType,
                CreatedBy = s.CreatedBy,
                CreatedDate = s.CreatedDate.ToShortDateString()
            }).ToListAsync();

            return Ok(new { data = services });
        }

        [HttpPost]
        public async Task<IActionResult> SaveService(ServiceViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var serviceInDb = await _context.Services.FirstOrDefaultAsync(c => c.Id == model.Service.Id && c.IsActive && !c.IsDelete);
                    if (serviceInDb == null)
                    {

                        var service = new Service
                        {
                            Name = model.Service.Name,
                            EnableTwoStepBooking = model.Service.EnableTwoStepBooking,
                            ServiceType = (int)model.Service.ServiceType,
                            CreatedBy = GetUserId(),
                            CreatedDate = Defaults.GetDateTime(),
                            ModifiedBy = GetUserId(),
                            ModifiedDate = Defaults.GetDateTime(),
                            IsActive = true
                        };
                        await _context.Services.AddAsync(service);
                    }
                    else
                    {
                        serviceInDb.Name = model.Service.Name;
                        serviceInDb.EnableTwoStepBooking = model.Service.EnableTwoStepBooking;
                        serviceInDb.ServiceType = (int)model.Service.ServiceType;
                        serviceInDb.ModifiedBy = GetUserId();
                        serviceInDb.ModifiedDate = Defaults.GetDateTime();
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
        public async Task<IActionResult> Delete(int id)
        {
            var result = false;
            var errMsg = string.Empty;
            try
            {
                var service = await _context.Services.FindAsync(id);
                if (service == null)
                    throw new InvalidOperationException("Invalid Service Id");
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
