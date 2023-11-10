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
    public class WebsiteController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _serviceGalleryPath;
        private readonly string _websiteSettingPath;
        public WebsiteController(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext, IWebHostEnvironment webHostEnvironment) : base(userManager)
        {
            _context = applicationDbContext;
            _webHostEnvironment = webHostEnvironment;
            _serviceGalleryPath = _webHostEnvironment.WebRootPath + AppConfig.ServiceGalleryImageUrl.Replace("/", "\\");
            _websiteSettingPath = _webHostEnvironment.WebRootPath + AppConfig.WebsiteSettingsImageUrl.Replace("/", "\\");
        }

        public async Task<IActionResult> Index()
        {
            return View(new WebsiteSettingViewModel
            {
                WebsiteSetting = await WebsiteSettingDTO.GetWebsiteSettingAsync(_context) ?? new WebsiteSettingDTO()
            });
        }

        public IActionResult ServiceGallery()
        {
            return View();
        }

        public async Task<IActionResult> CreateServiceGallery()
        {
            var serviceGalleryViewModel = new ServiceGalleryViewModel();
            serviceGalleryViewModel.ServiceGallery.OrderId = await _context.ServiceGallery.Where(s => s.IsActive && !s.IsDelete).MaxAsync(s => s.OrderId);
            return View(serviceGalleryViewModel);
        }

        public async Task<IActionResult> EditServiceGallery(int id)
        {
            var serviceGallery = await ServiceGalleryDTO.GetServiceGallery(_context, id);
            var serviceGalleryViewModel = new ServiceGalleryViewModel
            {
                ServiceGallery = serviceGallery
            };
            return View("CreateServiceGallery", serviceGalleryViewModel);
        }

        public async Task<IActionResult> ServiceGalleryList()
        {
            var services = await _context.ServiceGallery.Where(c => c.IsActive && !c.IsDelete).Select(s => new ServiceGalleryDTO
            {
                Id = s.Id,
                OrderId = s.OrderId,
                Title = s.Title,
                SubTitle = s.SubTitle,
                Link = s.Link,
                Image = AppConfig.ServiceGalleryImageUrl + s.Image,
                CreatedBy = s.CreatedBy,
                IsActive = s.IsActive,
                CreatedDate = s.CreatedDate.ToShortDateString()
            }).ToListAsync();

            return Ok(new { data = services });
        }

        [HttpPost]
        public async Task<IActionResult> Update(WebsiteSettingViewModel model)
        {
            try
            {
                var websiteSettings = await _context.WebsiteSettings.FirstOrDefaultAsync();
                if (websiteSettings == null)
                {
                    var websiteSetting = new WebsiteSetting
                    {
                        Name = model.WebsiteSetting.Name,
                        TagLine = model.WebsiteSetting.TagLine,
                        Email = model.WebsiteSetting.Email,
                        Mobile = model.WebsiteSetting.Mobile,
                        Address = model.WebsiteSetting.Address,
                        GoogleMapUrl = model.WebsiteSetting.GoogleMapUrl,
                        CopyRightText = model.WebsiteSetting.CopyRightText,
                        Year = model.WebsiteSetting.Year,
                        FacebookLink = model.WebsiteSetting.FacebookLink,
                        InstagramLink = model.WebsiteSetting.InstagramLink,
                        TwitterLink = model.WebsiteSetting.TwitterLink,
                        LinkedinLink = model.WebsiteSetting.LinkedinLink,
                        DiscordLink = model.WebsiteSetting.DiscordLink,
                        EmailAccount = model.WebsiteSetting.EmailAccount,
                        EmailAccountUserId = model.WebsiteSetting.EmailAccountUserId,
                        EmailAccountPassword = model.WebsiteSetting.EmailAccountPassword,
                        EmailAccountSmtp = model.WebsiteSetting.EmailAccountSmtp,
                        Logo = model.WebsiteSetting.LogoName != null ? new Common().UploadedFile(model.WebsiteSetting.LogoName, _websiteSettingPath) : string.Empty,
                        FooterLogo = model.WebsiteSetting.FooterLogoName != null ? new Common().UploadedFile(model.WebsiteSetting.FooterLogoName, _websiteSettingPath) : string.Empty,
                        Favicon = model.WebsiteSetting.FaviconName != null ? new Common().UploadedFile(model.WebsiteSetting.FaviconName, _websiteSettingPath) : string.Empty
                    };
                    await _context.WebsiteSettings.AddAsync(websiteSetting);
                }
                else
                {
                    websiteSettings.Name = model.WebsiteSetting.Name;
                    websiteSettings.TagLine = model.WebsiteSetting.TagLine;
                    websiteSettings.Email = model.WebsiteSetting.Email;
                    websiteSettings.Mobile = model.WebsiteSetting.Mobile;
                    websiteSettings.Address = model.WebsiteSetting.Address;
                    websiteSettings.GoogleMapUrl = model.WebsiteSetting.GoogleMapUrl;
                    websiteSettings.CopyRightText = model.WebsiteSetting.CopyRightText;
                    websiteSettings.Year = model.WebsiteSetting.Year;
                    websiteSettings.FacebookLink = model.WebsiteSetting.FacebookLink;
                    websiteSettings.InstagramLink = model.WebsiteSetting.InstagramLink;
                    websiteSettings.TwitterLink = model.WebsiteSetting.TwitterLink;
                    websiteSettings.LinkedinLink = model.WebsiteSetting.LinkedinLink;
                    websiteSettings.DiscordLink = model.WebsiteSetting.DiscordLink;
                    websiteSettings.EmailAccount = model.WebsiteSetting.EmailAccount;
                    websiteSettings.EmailAccountUserId = model.WebsiteSetting.EmailAccountUserId;
                    websiteSettings.EmailAccountPassword = model.WebsiteSetting.EmailAccountPassword;
                    websiteSettings.EmailAccountSmtp = model.WebsiteSetting.EmailAccountSmtp;
                    websiteSettings.Logo = model.WebsiteSetting.LogoName != null ? new Common().UploadedFile(model.WebsiteSetting.LogoName, _websiteSettingPath) : websiteSettings.Logo;
                    websiteSettings.FooterLogo = model.WebsiteSetting.FooterLogoName != null ? new Common().UploadedFile(model.WebsiteSetting.FooterLogoName, _websiteSettingPath) : websiteSettings.FooterLogo;
                    websiteSettings.Favicon = model.WebsiteSetting.FaviconName != null ? new Common().UploadedFile(model.WebsiteSetting.FaviconName, _websiteSettingPath) : websiteSettings.Favicon;
                }
                await _context.SaveChangesAsync();
                model.Result = true;
            }
            catch (Exception ex)
            {
                model.ErrorMessage = ex.Message;
                model.Result = false; ;
            }
            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveServiceGallery(ServiceGalleryViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var serviceGalleryInDb = await _context.ServiceGallery.FirstOrDefaultAsync(c => c.Id == model.ServiceGallery.Id && !c.IsDelete);
                    if (serviceGalleryInDb == null)
                    {

                        var serviceGallery = new ServiceGallery
                        {
                            OrderId = model.ServiceGallery.OrderId,
                            Title = model.ServiceGallery.Title,
                            SubTitle = model.ServiceGallery.SubTitle,
                            Link = model.ServiceGallery.Link,
                            IsActive = true,
                            CreatedBy = GetUserId(),
                            CreatedDate = Defaults.GetDateTime(),
                            ModifiedBy = GetUserId(),
                            ModifiedDate = Defaults.GetDateTime(),
                            Image = model.ServiceGallery.ImageName != null ? new Common().UploadedFile(model.ServiceGallery.ImageName, _serviceGalleryPath) : string.Empty,
                        };
                        await _context.ServiceGallery.AddAsync(serviceGallery);
                    }
                    else
                    {
                        serviceGalleryInDb.OrderId = model.ServiceGallery.OrderId;
                        serviceGalleryInDb.Title = model.ServiceGallery.Title;
                        serviceGalleryInDb.SubTitle = model.ServiceGallery.SubTitle;
                        serviceGalleryInDb.Link = model.ServiceGallery.Link;
                        serviceGalleryInDb.IsActive = model.ServiceGallery.IsActive;
                        serviceGalleryInDb.ModifiedBy = GetUserId();
                        serviceGalleryInDb.ModifiedDate = Defaults.GetDateTime();
                        serviceGalleryInDb.Image = model.ServiceGallery.ImageName != null ? new Common().UploadedFile(model.ServiceGallery.ImageName, _serviceGalleryPath) : serviceGalleryInDb.Image;
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
            return View("CreateServiceGallery", model);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteServiceGallery(int id)
        {
            var result = false;
            var errMsg = string.Empty;
            try
            {
                var serviceGallery = await _context.ServiceGallery.FindAsync(id);
                if (serviceGallery == null)
                    throw new InvalidOperationException("Invalid Service Gallery Id");
                serviceGallery.IsActive = false;
                serviceGallery.IsDelete = true;
                serviceGallery.ModifiedBy = GetUserId();
                serviceGallery.ModifiedDate = Defaults.GetDateTime();
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
