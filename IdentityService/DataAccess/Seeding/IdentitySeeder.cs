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
        else if ((await db.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
        {
            throw new InvalidOperationException(
                "Apply the IdentityService migrations before startup or enable Database:ApplyMigrationsOnStartup.");
        }

        await SeedRolesAsync(roleManager);
    }

    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var name in new[] { RoleNames.User, RoleNames.Admin })
        {
            if (await roleManager.RoleExistsAsync(name))
                continue;

            var role = new IdentityRole(name);
            try
            {
                var result = await roleManager.CreateAsync(role);
                if (!result.Succeeded && !await roleManager.RoleExistsAsync(name))
                    throw new InvalidOperationException($"Unable to seed the {name} role.");
            }
            catch (DbUpdateException ex) when (ex.Entries.Count == 1 && ex.Entries[0].Entity == role)
            {
                // Another instance may have inserted the role after the existence check.
                if (!await roleManager.RoleExistsAsync(name))
                    throw;

                ex.Entries[0].State = EntityState.Detached;
            }
        }
    }
}
