using Microsoft.AspNetCore.Identity;

namespace P7CreateRestApi.Domain
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
