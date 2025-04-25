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
            string[] roleNames = { "Admin", "Mechanic", "Client" };

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

            //// Szerelő felhasználó
            //var mechanicEmail = "mechanic@autoszerelo.hu";
            //var mechanicUser = await userManager.FindByEmailAsync(mechanicEmail);
            //if (mechanicUser == null)
            //{
            //    var newUser = new ApplicationUser
            //    {
            //        UserName = mechanicEmail,
            //        Email = mechanicEmail,
            //        EmailConfirmed = true
            //    };

            //    string mechanicPassword = "Mechanic123!";
            //    var createResult = await userManager.CreateAsync(newUser, mechanicPassword);

            //    if (createResult.Succeeded)
            //    {
            //        await userManager.AddToRoleAsync(newUser, "Mechanic");
            //    }
            //}

            // Create a list of mechanic data
            var mechanics = new List<(string Email, string PhoneNumber, string Password, string FirstName, string LastName)>
                {
                    ("mechanic1@autoszerelo.hu", "06 20 1112222", "Mechanic123!", "János", "Kovács"),
                    ("mechanic2@autoszerelo.hu", "06 20 3334444", "Mechanic123!", "Péter", "Nagy")
                };

            // Loop through the list and create each mechanic
            foreach (var mechanic in mechanics)
            {
                var mechanicUser = await userManager.FindByEmailAsync(mechanic.Email);
                if (mechanicUser == null)
                {
                    var newUser = new ApplicationUser
                    {
                        UserName = mechanic.Email,
                        Email = mechanic.Email,
                        EmailConfirmed = true,
                        PhoneNumber = mechanic.PhoneNumber,
                        UserFirstNames = mechanic.FirstName,
                        UserLastName = mechanic.LastName
                    };

                    var createResult = await userManager.CreateAsync(newUser, mechanic.Password);
                    if (createResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newUser, "Mechanic");
                    }
                    else
                    {
                        // Handle any errors during user creation
                        var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                        // Log errors or handle them as needed
                    }
                }
            }

            // Ügyfél felhasználó
            var clientEmail = "client01@autoszerelo.hu";
            var clientUser = await userManager.FindByEmailAsync(clientEmail);
            if (clientUser == null)
            {
                var newUser = new ApplicationUser
                {
                    UserName = clientEmail,
                    Email = clientEmail,
                    EmailConfirmed = true,
                    PhoneNumber = "0620 1111111"
                };

                string clientPassword = "Client123!";
                var createResult = await userManager.CreateAsync(newUser, clientPassword);

                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, "Client");
                }
            }
        }
    }
}
