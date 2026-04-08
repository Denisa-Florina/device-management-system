using DeviceManagement.Domain.Entities;
using DeviceManagement.Domain.Interfaces;
using DeviceManagement.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace DeviceManagement.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        IUserRepository userRepository)
    {
        foreach (var role in new[] { "Admin", "Customer" })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        const string adminEmail = "admin@email.com";
        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var domainUser = new User { Name = "Admin", Role = "Admin", Location = "HQ" };
            var createdDomainUser = await userRepository.CreateAsync(domainUser);

            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                UserId = createdDomainUser.Id
            };

            await userManager.CreateAsync(admin, "Admin@123!");
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
