using Microsoft.AspNetCore.Identity;

namespace Car_Rental.Data.UserManagment
{
    public class IdentitySeedData
    {
        public static async Task SeedAsync(
            IServiceProvider services,
            IConfiguration configuration)
        {
            using var scope = services.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();


            string[] roleNames = { "Admin", "Staff", "User" };

            foreach (var roleName in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            
            var adminEmail = configuration["AdminUser:Email"] ?? "admin@carrental.local";
            var adminPassword = configuration["AdminUser:Password"] ?? "Admin123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(adminUser, adminPassword);
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    // In a real app, log this
                    // for now, you can set a breakpoint and inspect errors
                }
            }
        }
    }
}
