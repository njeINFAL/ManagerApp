using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backend
{
    public class DBInitializer
    {
        public static async Task SeedDemoAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var context = services.GetRequiredService<ApplicationDbContext>();

            await context.Database.MigrateAsync();

            // Ensure "Mechanic" role exists
            if (!await roleManager.RoleExistsAsync("Mechanic"))
            {
                await roleManager.CreateAsync(new IdentityRole("Mechanic"));
            }

            // Create a mechanic user
            var mechanicEmail = "mechanic@demo.com";
            var mechanic = await userManager.FindByEmailAsync(mechanicEmail);
            if (mechanic == null)
            {
                mechanic = new ApplicationUser
                {
                    UserName = "demoMechanic",
                    Email = mechanicEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(mechanic, "Demo2025!"); 
                await userManager.AddToRoleAsync(mechanic, "Mechanic");
            }

            // Add availability for all weekdays
            var existingAvailability = context.MechanicAvailabilities
                .Where(a => a.ApplicationUserId == mechanic.Id);
            context.MechanicAvailabilities.RemoveRange(existingAvailability); // Clean slate

            for (var day = DayOfWeek.Monday; day <= DayOfWeek.Friday; day++)
            {
                context.MechanicAvailabilities.Add(new MechanicAvailability
                {
                    ApplicationUserId = mechanic.Id,
                    DayOfWeek = day,
                    StartTime = TimeSpan.FromHours(8),
                    EndTime = TimeSpan.FromHours(16)
                });
            }

            await context.SaveChangesAsync();
        }
    }
}
