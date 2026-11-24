using IdentityService.Domain;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.DataAccess.Seeding;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var name in new[] { RoleNames.User, RoleNames.Admin })
        {
            if (await roleManager.RoleExistsAsync(name))
                continue;

            var result = await roleManager.CreateAsync(new IdentityRole(name));
            if (!result.Succeeded)
                throw new InvalidOperationException($"Unable to seed the {name} role.");
        }

    }
}
