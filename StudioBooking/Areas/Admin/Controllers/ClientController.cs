using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
    public class ClientController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _filePath;
        public ClientController(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext, IWebHostEnvironment webHostEnvironment) : base(userManager)
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
            return View(new ClientViewModel());
        }

        public async Task<IActionResult> Edit(int id)
        {
            var clientViewModel = new ClientViewModel
            {
                Client = await ClientDTO.GetClient(_context, id)
            };
            return View("Create", clientViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SaveClient(ClientViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var clientInDb = await _context.Clients.FirstOrDefaultAsync(c => c.Id == model.Client.Id && !c.IsDelete);
                    if (clientInDb == null)
                    {

                        var client = new Client
                        {
                            Name = model.Client.Name,
                            Order = model.Client.Order,
                            EnablePulse = model.Client.EnablePulse,
                            Summary = model.Client.Summary,
                            CreatedBy = GetUserId(),
                            CreatedDate = Defaults.GetDateTime(),
                            ModifiedBy = GetUserId(),
                            ModifiedDate = Defaults.GetDateTime(),
                            IsActive = true,
                            Image = model.Client.ImageName != null ? new Common().UploadedFile(model.Client.ImageName, _filePath) : string.Empty,
                        };
                        await _context.Clients.AddAsync(client);
                    }
                    else
                    {
                        clientInDb.Name = model.Client.Name;
                        clientInDb.Order = model.Client.Order;
                        clientInDb.EnablePulse = model.Client.EnablePulse;
                        clientInDb.Summary = model.Client.Summary;
                        clientInDb.ModifiedBy = GetUserId();
                        clientInDb.ModifiedDate = Defaults.GetDateTime();
                        clientInDb.Image = model.Client.ImageName != null ? new Common().UploadedFile(model.Client.ImageName, _filePath) : clientInDb.Image;
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
        public async Task<IActionResult> GetClients()
        {
            var clients = await ClientDTO.GetClients(_context, true);
            return Ok(new { data = clients });
        }
    }
}
