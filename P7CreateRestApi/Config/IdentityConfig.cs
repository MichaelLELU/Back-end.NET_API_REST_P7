using Microsoft.AspNetCore.Identity;
using P7CreateRestApi.Data;
using P7CreateRestApi.Domain;


namespace P7CreateRestApi.Config
{ 
  public static class IdentityConfig
{
    public static IServiceCollection AddAppIdentity(this IServiceCollection services)
    {
        services.AddIdentity<AppUser, IdentityRole>(options =>
        {
                // --- Paramètres utilisateur ---
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@";

            // --- Paramètres des mots de passe ---
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;

            // --- Paramètres de verrouillage ---
            options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
            })
            .AddEntityFrameworkStores<LocalDbContext>()
            .AddDefaultTokenProviders();

            return services;
        }
    }
}
