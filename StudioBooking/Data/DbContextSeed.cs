using Microsoft.AspNetCore.Identity;
using StudioBooking.Data.Models;
using StudioBooking.Infrastructure;

namespace StudioBooking.Data
{
    public class DbContextSeed
    {
        public static async Task SeedRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Enums.Roles.SuperAdmin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Enums.Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Enums.Roles.Moderator.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Enums.Roles.Basic.ToString()));
        }

        public static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Default User
            var defaultUser = new ApplicationUser
            {
                UserName = "anup1421@gmail.com",
                Email = "anup1421@gmail.com",
                FirstName = "Anup",
                LastName = "Pandey",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                CreatedDate= Defaults.GetDateTime()                
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Admin@1421");
                    await userManager.AddToRoleAsync(defaultUser, Enums.Roles.Basic.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Enums.Roles.Moderator.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Enums.Roles.Admin.ToString());
                    await userManager.AddToRoleAsync(defaultUser, Enums.Roles.SuperAdmin.ToString());
                }                
            }

            //Super Admin 2
            var defaultUser1 = new ApplicationUser
            {
                UserName = "admin@rbstudios.info",
                Email = "admin@rbstudios.info",
                FirstName = "Studio",
                LastName = "Admin",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                CreatedDate = Defaults.GetDateTime()
            };
            if (userManager.Users.All(u => u.Id != defaultUser1.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser1.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser1, "Admin$#123");
                    await userManager.AddToRoleAsync(defaultUser1, Enums.Roles.Basic.ToString());
                    await userManager.AddToRoleAsync(defaultUser1, Enums.Roles.Moderator.ToString());
                    await userManager.AddToRoleAsync(defaultUser1, Enums.Roles.Admin.ToString());
                    await userManager.AddToRoleAsync(defaultUser1, Enums.Roles.SuperAdmin.ToString());
                }
            }
        }
    }
}
