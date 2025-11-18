using Microsoft.AspNetCore.Identity;
using P7CreateRestApi.Constants;

namespace P7CreateRestApi.Services
{
    public static class RoleInitializer
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            foreach (var roleName in AppRoles.AllowedRoles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            string adminEmail = "admin@findexium.com";
            string adminUserName = "admin";
            string adminPassword = "Admin123456789!";

            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);

            if (existingAdmin == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = adminUserName,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                    await userManager.AddToRoleAsync(adminUser, AppRoles.Admin);
            }
        }
    }
}
