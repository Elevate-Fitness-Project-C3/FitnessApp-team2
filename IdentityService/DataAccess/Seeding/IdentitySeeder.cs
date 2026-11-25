using IdentityService.DataAccess.Context;
using IdentityService.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.DataAccess.Seeding;

public static class IdentitySeeder
{
    public static async Task InitializeAsync(ApplicationDbContext db, RoleManager<IdentityRole> roleManager,
        bool applyMigrations, CancellationToken cancellationToken = default)
    {
        if (applyMigrations)
        {
            await db.Database.MigrateAsync(cancellationToken);
        }

        await SeedRolesAsync(roleManager);
    }

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

