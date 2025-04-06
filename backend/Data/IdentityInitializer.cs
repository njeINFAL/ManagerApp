using Microsoft.AspNetCore.Identity;
using backend.Models;

namespace backend.Data
{
    public static class IdentityInitializer
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Szerepkörök
            string[] roleNames = { "Admin", "Szerelő", "Ügyfél" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Admin felhasználó
            var adminEmail = "admin@autoszerelo.hu";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var newUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                string adminPassword = "Admin123!"; 
                var createResult = await userManager.CreateAsync(newUser, adminPassword);

                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, "Admin");
                }
            }
        }
    }
}
