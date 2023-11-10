using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using StudioBooking.Areas.Admin.Models;
using StudioBooking.Data;
using StudioBooking.Data.Models;
using StudioBooking.DTO;
using StudioBooking.Infrastructure;

namespace StudioBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CategoryController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _filePath;
        public CategoryController(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext, IWebHostEnvironment webHostEnvironment) : base(userManager)
        {
            _context = applicationDbContext;
            _webHostEnvironment = webHostEnvironment;
            _filePath = _webHostEnvironment.WebRootPath + AppConfig.CategoryImageUrl.Replace("/", "\\");
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            var categoryViewModel = new CategoryViewModel
            {
                FilePath = _filePath
            };
            return View(categoryViewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var categoryViewModel = new CategoryViewModel
            {
                Category = await CategoryDTO.GetCategory(_context, id),
                FilePath = _filePath
            };
            return View("Create", categoryViewModel);
        }

        public async Task<IActionResult> CategoryDetail(int id)
        {
            var categoryDetailViewModel = new CategoryDetailViewModel
            {
                Category = await CategoryDTO.GetCategory(_context, id),
                FilePath = _filePath
            };

            var categoryDetailInDb = await CategoryDetailDTO.GetCategoryDetails(_context, id);
            //Check if Details Exists
            if (categoryDetailInDb.Count == 0)
            {

                categoryDetailViewModel.CategoryLiveRoom.CategoryId = id;
                categoryDetailViewModel.CategoryLiveRoom.Type = Enums.DetailType.LiveRoom;
                categoryDetailViewModel.CategoryControlRoom.CategoryId = id;
                categoryDetailViewModel.CategoryControlRoom.Type = Enums.DetailType.ControlRoom;
            }
            else
            {
                foreach (var categoryDetail in categoryDetailInDb)
                {
                    if (categoryDetail.Type == Enums.DetailType.LiveRoom)
                        categoryDetailViewModel.CategoryLiveRoom = categoryDetail;
                    if (categoryDetail.Type == Enums.DetailType.ControlRoom)
                        categoryDetailViewModel.CategoryControlRoom = categoryDetail;
                }
            }
            return View(categoryDetailViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> CreateGear(int id, int detailId)
        {
            if (detailId == 0)
                return PartialView("_CreateGearPartial", new CategoryGearDTO());
            else
            {
                var categoryGear = await _context.CategoryGears.FirstOrDefaultAsync(s => s.Id == id && !s.IsDelete);
                if (categoryGear == null)
                    return PartialView("_CreateGearPartial", new CategoryGearDTO { CategoryDetailId = detailId });
                return PartialView("_CreateGearPartial", new CategoryGearDTO
                {
                    Id = categoryGear.Id,
                    CategoryDetailId = categoryGear.CategoryDetailId,
                    Content = categoryGear.Content,
                    Type = (Enums.GearContentType)(categoryGear.Type ?? 0),
                    Icon = categoryGear.Icon,
                    Image = categoryGear.Image
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoryList()
        {
            var categoryList = await _context.Categories.Include(c => c.ServicePrices).Where(c => c.IsActive && !c.IsDelete).Select(s => new CategoryDTO
            {
                Id = s.Id,
                Name = s.Name,
                Title = s.Title,
                Image = AppConfig.CategoryImageUrl + s.Image,
                Description = s.Description,
                DisableBooking = !s.ServicePrices.Any(c => c.DisableBooking != true),
                IsActive = s.IsActive,
                CreatedBy = s.CreatedBy,
                CreatedDate = s.CreatedDate.ToShortDateString()
            }).ToListAsync();

            return Ok(new { data = categoryList });
        }

        [HttpPost]
        public async Task<IActionResult> AddCategoryGear(CategoryGearDTO model)
        {
            if (model.Id > 0)
            {
                var categoryGear = await _context.CategoryGears.FirstOrDefaultAsync(c => c.Id == model.Id && !c.IsDelete);
                if (categoryGear != null)
                {
                    categoryGear.Content = model.Content;
                    categoryGear.Type = (int)(model.Type ?? Enums.GearContentType.Basic);
                    categoryGear.Icon = model.Icon;
                    categoryGear.Image = model.Image;
                    categoryGear.ModifiedBy = GetUserId();
                    categoryGear.ModifiedDate = Defaults.GetDateTime();
                    await _context.SaveChangesAsync();
                }
            }
            return PartialView("_GearPartial", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveCategory(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var categoryInDb = await _context.Categories.FirstOrDefaultAsync(c => c.Id == model.Category.Id && !c.IsDelete);
                    if (categoryInDb == null)
                    {

                        var category = new Category
                        {
                            Name = model.Category.Name,
                            Title = model.Category.Title,
                            StartTime = model.Category.StartTime,
                            EndTime = model.Category.EndTime,
                            Description = model.Category.Description,
                            CreatedBy = GetUserId(),
                            CreatedDate = Defaults.GetDateTime(),
                            ModifiedBy = GetUserId(),
                            ModifiedDate = Defaults.GetDateTime(),
                            IsActive = true,
                            Image = model.Category.ImageName != null ? new Common().UploadedFile(model.Category.ImageName, _filePath) : string.Empty,
                        };
                        await _context.Categories.AddAsync(category);
                    }
                    else
                    {
                        categoryInDb.Name = model.Category.Name;
                        categoryInDb.Title = model.Category.Title;
                        categoryInDb.StartTime = model.Category.StartTime;
                        categoryInDb.EndTime = model.Category.EndTime;
                        categoryInDb.Description = model.Category.Description;
                        categoryInDb.ModifiedBy = GetUserId();
                        categoryInDb.ModifiedDate = Defaults.GetDateTime();
                        categoryInDb.Image = model.Category.ImageName != null ? new Common().UploadedFile(model.Category.ImageName, _filePath) : categoryInDb.Image;
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

        [HttpPost]
        public async Task<IActionResult> SaveCategoryDetail(CategoryDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Save Live Room Details
                    var categoryLiveRoomDetail = await CategoryDetailDTO.SaveCategoryDetail(_context, model.CategoryLiveRoom, GetUserId());
                    //Save Live Room Gears
                    CategoryGearDTO.SaveGearDetails(_context, categoryLiveRoomDetail ?? new CategoryDetail(), model.CategoryLiveRoom.CategoryGears);
                    //Save Control Room Details
                    var categoryControlRoomDetail = await CategoryDetailDTO.SaveCategoryDetail(_context, model.CategoryControlRoom, GetUserId());
                    //Save Live Room Gears
                    CategoryGearDTO.SaveGearDetails(_context, categoryLiveRoomDetail ?? new CategoryDetail(), model.CategoryControlRoom.CategoryGears);
                    model.Result = true;
                }
                catch (Exception ex)
                {
                    model.Result = false;
                    model.ErrorMessage = ex.Message;
                }
            }
            return View("CategoryDetail", model);
        }

        [HttpGet]
        public async Task<IActionResult> ToggleBookingStatus(int id, bool status)
        {
            var result = false;
            var errMsg = string.Empty;
            try
            {
                var category = await _context.Categories.Include(c => c.ServicePrices).FirstOrDefaultAsync(c => c.Id == id) ?? throw new InvalidOperationException("Invalid Category ID");
                foreach (var servicePrice in category.ServicePrices)
                {
                    servicePrice.DisableBooking = !status;
                    servicePrice.ModifiedBy = GetUserId();
                    servicePrice.ModifiedDate = Defaults.GetDateTime();
                }
                category.ModifiedBy = GetUserId();
                category.ModifiedDate = Defaults.GetDateTime();
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
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                    throw new InvalidOperationException("Invalid Category ID");
                category.IsActive = false;
                category.IsDelete = true;
                category.ModifiedBy = GetUserId();
                category.ModifiedDate = Defaults.GetDateTime();
                await ServicePrice.DeleteCategoryServicePrices(_context, category.Id, GetUserId());
                await Data.Models.CategoryDetail.DeleteCategoryDetail(_context, category.Id, GetUserId());
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
        public async Task<IActionResult> DeleteCategoryGear(int id)
        {
            var result = false;
            var errMsg = string.Empty;
            try
            {
                var categoryGear = await _context.CategoryGears.FindAsync(id);
                if (categoryGear == null)
                    throw new InvalidOperationException("Invalid Category Gear ID");
                categoryGear.IsActive = false;
                categoryGear.IsDelete = true;
                categoryGear.ModifiedBy = GetUserId();
                categoryGear.ModifiedDate = Defaults.GetDateTime();
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
