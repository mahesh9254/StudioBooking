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
    public class TeamController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _filePath;
        public TeamController(UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext, IWebHostEnvironment webHostEnvironment) : base(userManager)
        {
            _context = applicationDbContext;
            _webHostEnvironment = webHostEnvironment;
            _filePath = _webHostEnvironment.WebRootPath + AppConfig.TeamImageUrl.Replace("/", "\\");
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View(new TeamViewModel());
        }

        public async Task<IActionResult> Edit(int id)
        {
            var teamViewModel = new TeamViewModel
            {
                Team = await TeamDTO.GetTeam(_context, id)
            };
            return View("Create", teamViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SaveTeam(TeamViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var teamInDb = await _context.Teams.FirstOrDefaultAsync(c => c.Id == model.Team.Id && !c.IsDelete);
                    if (teamInDb == null)
                    {

                        var team = new Team
                        {
                            Name = model.Team.Name,
                            Professions = model.Team.Professions,
                            Description = model.Team.Description,
                            CreatedBy = GetUserId(),
                            CreatedDate = Defaults.GetDateTime(),
                            ModifiedBy = GetUserId(),
                            ModifiedDate = Defaults.GetDateTime(),
                            IsActive = true,
                            Image = model.Team.ImageName != null ? new Common().UploadedFile(model.Team.ImageName, _filePath) : string.Empty,
                        };
                        await _context.Teams.AddAsync(team);
                    }
                    else
                    {
                        teamInDb.Name = model.Team.Name;
                        teamInDb.Description = model.Team.Description;
                        teamInDb.Professions = model.Team.Professions;
                        teamInDb.ModifiedBy = GetUserId();
                        teamInDb.ModifiedDate = Defaults.GetDateTime();
                        teamInDb.Image = model.Team.ImageName != null ? new Common().UploadedFile(model.Team.ImageName, _filePath) : teamInDb.Image;
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
        public async Task<IActionResult> GetTeams()
        {
            var teams = await TeamDTO.GetTeams(_context, true);
            return Ok(new { data = teams });
        }
    }
}
