using Microsoft.AspNetCore.Identity;

namespace LibrAI.Services
{
    public class SeedRoles
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Roller kontrol edilir ve yoksa eklenir
            string[] roleNames = { "Admin", "Kütüphaneci", "Öğrenci" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    var role = new IdentityRole(roleName);
                    await roleManager.CreateAsync(role);
                }
            }

            // Admin rolüne sahip bir kullanıcı eklemek
            var user = await userManager.FindByEmailAsync("admin@librai.com");
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = "admin@librai.com",
                    Email = "admin@librai.com"
                };
                await userManager.CreateAsync(user, "Password123!");
            }
            if (!await userManager.IsInRoleAsync(user, "Admin"))
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}
