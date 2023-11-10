using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudioBooking.Areas.Admin.Models;
using StudioBooking.Data.Models;
using StudioBooking.DTO;
using StudioBooking.Infrastructure;
using StudioBooking.ViewModels;
using StudioBooking.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using static StudioBooking.Infrastructure.Enums;

namespace StudioBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly SignInManager<ApplicationUser> _signManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<AccountController> _logger;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signManager, RoleManager<IdentityRole> roleManager, IUserStore<ApplicationUser> userStore, ApplicationDbContext applicationDbContext, ILogger<AccountController> logger) : base(userManager)
        {
            _userManager = userManager;
            _signManager = signManager;
            _roleManager = roleManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _context = applicationDbContext;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Customers()
        {
            return View();
        }

        public async Task<IActionResult> Add()
        {
            var accountViewModel = new AccountViewModel
            {
                Roles = new SelectList(await GetRoles(), nameof(IdentityRole.Name), nameof(IdentityRole.Name))
            };
            return View("Create", accountViewModel);
        }

        public IActionResult Create()
        {
            var accountViewModel = new AccountViewModel();
            accountViewModel.User.Role = UserRole.Basic;
            return View(accountViewModel);
        }

        public async Task<IActionResult> Update(string id)
        {
            var user = await UserDTO.GetUser(_context, id);
            var userRoles = await GetUserRoles(new ApplicationUser { Id = user.UserId });
            user.Role = userRoles.FirstOrDefault();
            var customer = userRoles.Contains(UserRole.Basic) ? await CustomerDTO.GetCustomer(_context, id) : new CustomerDTO();
            var accountViewModel = new AccountViewModel
            {
                User = user,
                Customer = customer,
                Roles = !userRoles.Contains(UserRole.Basic) ? new SelectList(await GetRoles(), nameof(IdentityRole.Name), nameof(IdentityRole.Name)) : null,
                Wallets = customer.Id > 0 ? await WalletDTO.GetCustomerWallets(_context, customer.Id) : new List<WalletDTO>(),
            };
            return View("Create", accountViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> UserList()
        {
            var result = false;
            var errMsg = string.Empty;
            try
            {
                var userRole = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == UserRole.SuperAdmin);
                var users = await _userManager.Users.Where(u=> u.Email != "anup1421@gmail.com").ToListAsync();
                var userViewModel = new List<UserViewModel>();
                foreach (ApplicationUser user in users)
                {
                    userViewModel.Add(new UserViewModel
                    {
                        User = new UserDTO
                        {
                            UserId = user.Id,
                            Email = user.Email,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Name = user.FirstName + " " + user.LastName,
                            ProfileImageUrl = user.ProfileImageUrl ?? Defaults.ProfileImage,
                        },
                        Roles = await GetUserRoles(user)
                    });
                }
                //if (User.IsInRole(UserRole.Admin))
                //{
                //    userViewModel = userViewModel.Where(u => !u.Roles.Any(u => u == UserRole.Basic)).ToList();
                //}
                return Json(new { users = userViewModel, result = true, errMsg });
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return Json(new { users = new List<UserViewModel>(), result, errMsg });
        }

        [HttpGet]
        public async Task<IActionResult> CustomerList()
        {
            var result = false;
            var errMsg = string.Empty;
            try
            {
                var customers = await _context.Customers.Include(c => c.User).Where(c => c.IsActive && !c.IsDelete).ToListAsync();
                var userViewModel = new List<UserViewModel>();
                foreach (var customer in customers)
                {
                    userViewModel.Add(new UserViewModel
                    {
                        User = new UserDTO
                        {
                            UserId = customer.User.Id,
                            CustomerId = customer.Id.ToString(Defaults.CustomerPrefix),
                            Email = customer.User.Email,
                            FirstName = customer.User.FirstName,
                            LastName = customer.User.LastName,
                            Name = customer.Name,
                            Mobile = customer.User.PhoneNumber,
                            CreatedDate = customer.User.CreatedDate.ToShortDateString(),
                            ProfileImageUrl = customer.User.ProfileImageUrl ?? Defaults.ProfileImage,
                        }
                        //Roles = await GetUserRoles(customer.User)
                    });
                }
                return Json(new { users = userViewModel, result = true, errMsg });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured at Admin/Account=> CustomerList : " + ex.Message);
                errMsg = ex.Message;
            }
            return Json(new { users = new List<UserViewModel>(), result, errMsg });
        }

        private async Task<List<string>> GetUserRoles(ApplicationUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }

        [HttpPost]
        public async Task<IActionResult> SaveUser(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(model.User.UserId))
                    {
                        using var transaction = _context.Database.BeginTransaction();
                        var user = await CreateUserAccount(model);
                        if (model.Result && model.User.Role == UserRole.Basic)
                        {
                            var customer = await Customer.AddCustomer(_context, user, model.Customer ?? new CustomerDTO(), GetUserId());
                            await CustomerAddress.AddAddress(_context, new CustomerAddress
                            {
                                CustomerId = customer.Id,
                                Name = user.FirstName + " " + user.LastName,
                                AddressLine1 = model.Customer.AddressLine1,
                                AddressLine2 = model.Customer.AddressLine2,
                                Landmark = model.Customer.Landmark,
                                City = model.Customer.City,
                                State = model.Customer.State,
                                PinCode = model.Customer.PinCode,
                                IsActive = true,
                                IsDefault = true,
                                CreatedDate = Defaults.GetDateTime()
                            });
                            await _context.Wallets.AddAsync(new Wallet
                            {
                                CustomerId = customer.Id,
                                WalletType = (int)WalletType.Points,
                                Mode = (int)WalletType.Points,
                                BalanceAmount = 0,
                                IsActive = true,
                                CreatedDate = Defaults.GetDateTime()
                            });
                        }                        
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        model.Result = true;
                    }
                    else
                    {
                        using var transaction = _context.Database.BeginTransaction();
                        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == model.User.UserId);
                        if (user == null)
                            throw new NullReferenceException("User not found, userId:" + model.User.UserId);

                        var userRoles = await GetUserRoles(user);
                        if (userRoles.FirstOrDefault() != model.User.Role)
                        {
                            foreach (var role in userRoles)
                            {
                                await _userManager.RemoveFromRoleAsync(user, role);
                            }
                            await _userManager.AddToRoleAsync(user, model.User.Role);
                        }

                        user.FirstName = model.User.FirstName;
                        user.LastName = model.User.LastName;
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
                        if (model.User.Role == UserRole.Basic)
                        {
                            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == model.Customer.Id);
                            if (customer == null)
                                throw new NullReferenceException("Customer not found, customerId:" + model.Customer.Id);

                            customer.Name = model.User.FirstName + " " + model.User.LastName;
                            customer.CompanyName = model.Customer.CompanyName;

                            var customerAddress = await _context.CustomerAddresses.FirstOrDefaultAsync(s => s.IsDefault && s.CustomerId == customer.Id);
                            customerAddress.CompanyName = model.Customer.CompanyName;
                            customerAddress.GstNumber = model.Customer.GstNumber;
                            customerAddress.AddressLine1 = model.Customer.AddressLine1;
                            customerAddress.AddressLine2 = model.Customer.AddressLine2;
                            customerAddress.Landmark = model.Customer.Landmark;
                            customerAddress.City = model.Customer.City;
                            customerAddress.State = model.Customer.State;
                            customerAddress.PinCode = model.Customer.PinCode;
                        }
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        model.Result = true;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occured at Admin=> SaveUser : " + ex.Message);
                    model.ErrorMessage = ex.Message;
                }
            }
            return View("Create", model);
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
                _logger.LogError(ex, "Error occured at Admin/Account=> UpdatePassword : " + ex.Message);
                model.ErrorMessage = ex.Message;
            }
            return View("Create", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetWalletBalanceUpdateModal(string userId)
        {
            try
            {
                var customerWallet = await _context.Wallets.Include(s => s.Customer).FirstOrDefaultAsync(c => c.Customer.UserId == userId && c.WalletType == (int)WalletType.Points) ?? throw new Exception("Customer wallet not found for userID: " + userId);
                var model = new WalletDTO
                {
                    Id = customerWallet.Id,
                    Mode = customerWallet.Mode,
                    BalanceAmount = customerWallet.BalanceAmount,
                    CustomerId = customerWallet.CustomerId,
                };
                return PartialView("_WalletBalanceUpdateModal", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured at Admin/Account=> GetWalletBalanceUpdateModal : " + ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateWallet(WalletDTO model)
        {
            var result = false;
            var errorMsg = string.Empty;
            try
            {
                var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.Id == model.Id && w.IsActive);
                if (wallet == null)
                {
                    wallet = new Wallet
                    {
                        CustomerId = model.CustomerId,
                        Mode = (int)WalletType.Points,
                        WalletType = (int)WalletType.Points,
                        IsActive = true,
                        CreatedBy = GetUserId(),
                        CreatedDate = DateTime.UtcNow
                    };
                    await _context.Wallets.AddAsync(wallet);
                    await _context.SaveChangesAsync();
                }
                await WalletDTO.AddUpdateWallet(_logger, _context, wallet, new WalletTransactionDTO { Amount = model.Amount, Description = model.Description, TransactionType = TransactionType.Credit, ReferenceId = "Added By Admin" }, GetUserId());
                await _context.SaveChangesAsync();
                result = true;
                return Ok(new { result, errorMsg });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured at Admin/Account=> UpdateWallet : " + ex.Message);
                return Ok(new { result, errorMsg });
            }
        }

        [HttpGet, ActionName("Delete")]
        public async Task<IActionResult> RemoveUser(string id)
        {
            var result = false;
            var errorMsg = string.Empty;
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                var rolesForUser = await _userManager.GetRolesAsync(user);
                var loginProvides = await _userManager.GetLoginsAsync(user);
                using var transaction = _context.Database.BeginTransaction();
                foreach (var login in loginProvides)
                {
                    await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
                }
                if (rolesForUser.Count > 0)
                {
                    foreach (var role in rolesForUser.ToList())
                    {
                        await _userManager.RemoveFromRoleAsync(user, role);
                    }
                }
                if (rolesForUser.FirstOrDefault() == UserRole.Basic)
                {
                    var customer = await _context.Customers.FirstOrDefaultAsync(c => c.IsActive && !c.IsDelete && c.UserId == user.Id);
                    customer.IsActive = false;
                    customer.IsDelete = true;
                    customer.ModifiedBy = GetUserId();
                    customer.ModifiedDate = Defaults.GetDateTime();
                }
                await _userManager.DeleteAsync(user);
                await _context.SaveChangesAsync();
                transaction.Commit();
                result = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured at Admin/Account=> RemoveUser : " + ex.Message);
                errorMsg = ex.Message;
            }
            return Ok(new { result, errorMsg });
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrors(result);
                }
            }
            return View();
        }

        private async Task<ApplicationUser> CreateUserAccount(AccountViewModel model)
        {
            var user = CreateUser();
            var email = string.IsNullOrEmpty(model.User.Email) ? "rb" + model.User.Mobile + "@rbstudios.info" : model.User.Email;
            await _userStore.SetUserNameAsync(user, model.User.Mobile ?? email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, email, CancellationToken.None);
            user.FirstName = model.User.FirstName;
            user.LastName = model.User.LastName;
            user.PhoneNumber = model.User.Mobile;
            user.CreatedBy = GetUserId();
            user.ModifiedBy = GetUserId();
            user.CreatedDate = Defaults.GetDateTime();
            user.ModifiedDate = Defaults.GetDateTime();

            var result = await _userManager.CreateAsync(user, model.User.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.User.Role);
                model.Result = true;
            }
            AddErrors(result);
            return user;
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }

        public async Task<List<IdentityRole>> GetRoles()
        {
            if (User.IsInRole(UserRole.SuperAdmin))
            {
                return await _roleManager.Roles.Where(r => r.Name != UserRole.SuperAdmin).ToListAsync();
            }
            else
            {
                return await _roleManager.Roles.Where(r => r.Name != UserRole.SuperAdmin && r.Name != UserRole.Basic).ToListAsync();
            }
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
