using Microsoft.AspNetCore.Identity;
using P7CreateRestApi.Domain;

public static class RoleInitializer
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();

        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));

        if (!await roleManager.RoleExistsAsync("User"))
            await roleManager.CreateAsync(new IdentityRole("User"));

        string email = "admin@findexium.com";
        string password = "Admin123456789!";

        var admin = await userManager.FindByEmailAsync(email);

        if (admin == null)
        {
            admin = new AppUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = "Administrator"
            };

            var result = await userManager.CreateAsync(admin, password);

            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
