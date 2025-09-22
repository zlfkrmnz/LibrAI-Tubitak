using Microsoft.AspNetCore.Identity;

namespace LibrAI.Data.Auth
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
