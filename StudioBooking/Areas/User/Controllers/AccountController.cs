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

namespace StudioBooking.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ApplicationDbContext _context;
        private readonly string _filePath;
        public AccountController(UserManager<ApplicationUser> userManager, IUserStore<ApplicationUser> userStore, ApplicationDbContext applicationDbContext, IWebHostEnvironment webHostEnvironment) : base(userManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _context = applicationDbContext;
            _filePath = webHostEnvironment.WebRootPath + AppConfig.ProfileImageUrl.Replace("/", "\\");
        }
        public async Task<IActionResult> Profile()
        {
            var accountViewModel = new AccountViewModel
            {
                User = await UserDTO.GetUser(_context, GetUserId()),
                Customer = await CustomerDTO.GetCustomer(_context, GetUserId())
            };
            return View(accountViewModel);
        }

        public async Task<IActionResult> ChangePassword()
        {
            var accountViewModel = new AccountViewModel
            {
                User = await UserDTO.GetUser(_context, GetUserId())
            };
            return View(accountViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SaveProfile(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using var transaction = _context.Database.BeginTransaction();
                    var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == model.User.UserId) ?? throw new NullReferenceException("User not found, userId:" + model.User.UserId);

                    user.FirstName = model.User.FirstName;
                    user.LastName = model.User.LastName;
                    user.ProfileImageUrl = model.User.ImageName != null ? new Common().UploadedFile(model.User.ImageName, _filePath) : user.ProfileImageUrl;
                    if (user.PhoneNumber != model.User.Mobile)
                    {
                        var checkDuplicate = _context.Users.Where(s => s.Id != model.User.UserId && s.PhoneNumber == model.User.Mobile).Count();
                        if (checkDuplicate > 0)
                            throw new InvalidOperationException("Mobile number already registered with another account");
                        await _userStore.SetUserNameAsync(user, model.User.Mobile, CancellationToken.None);
                        await _userStore.SetNormalizedUserNameAsync(user, model.User.Mobile, CancellationToken.None);
                        user.PhoneNumber = model.User.Mobile;
                    }
                    if (user.Email != model.User.Email)
                    {
                        var checkDuplicate = _context.Users.Where(s => s.Id != model.User.UserId && s.Email == model.User.Email).Count();
                        if (checkDuplicate > 0)
                            throw new InvalidOperationException("Email already registered with another account");

                        await _emailStore.SetEmailAsync(user, model.User.Email, CancellationToken.None);
                        await _emailStore.SetNormalizedUserNameAsync(user, model.User.Email, CancellationToken.None);
                        user.Email = model.User.Email;
                    }

                    var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == model.Customer.Id) ?? throw new NullReferenceException("Customer not found, customerId:" + model.Customer.Id);

                    customer.Name = model.User.FirstName + " " + model.User.LastName;
                    var customerAddress = await _context.CustomerAddresses.FirstOrDefaultAsync(s => s.IsDefault && s.CustomerId == customer.Id);
                    customerAddress.CompanyName = model.Customer.CompanyName;
                    customerAddress.GstNumber = model.Customer.GstNumber;
                    customerAddress.AddressLine1 = model.Customer.AddressLine1;
                    customerAddress.AddressLine2 = model.Customer.AddressLine2;
                    customerAddress.Landmark = model.Customer.Landmark;
                    customerAddress.City = model.Customer.City;
                    customerAddress.State = model.Customer.State;
                    customerAddress.PinCode = model.Customer.PinCode;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    model.Result = true;

                }
                catch (Exception ex)
                {
                    model.ErrorMessage = ex.Message;
                }
            }
            return View("Profile", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdatePassword(AccountViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.ErrorMessage = "Enter Valid Password";
                    return View("Create", model);
                }
                var user = await _userManager.FindByIdAsync(model.User.UserId);
                if (user == null)
                {
                    throw new InvalidOperationException("Opps something went wrong, Please contact admin.");
                }
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var identityResult = await _userManager.ResetPasswordAsync(user, token, model.User.Password);
                if (identityResult.Succeeded)
                {
                    //if (model.User.Role == UserRole.Basic)
                    //    if (Config.EnableSMS)
                    //    {
                    //        string smsTemplate = @"Hi, Your password is updated successfully at " + HttpContext.Request.Url.Host + ".\nNew Password: " + model.Password + "\nThank You!";
                    //        await UserManager.SendSmsAsync(user.Id, smsTemplate);
                    //    }
                    model.Result = true;
                }
                AddErrors(identityResult);
            }
            catch (Exception ex)
            {
                model.ErrorMessage = ex.Message;
            }
            return View("ChangePassword", model);
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}
