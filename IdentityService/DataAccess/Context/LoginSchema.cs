using Microsoft.EntityFrameworkCore;

namespace IdentityService.DataAccess.Context;

internal static class LoginSchema
{
    // EnsureCreated does not upgrade databases from previous development runs.
    public static async Task ApplyLoginSchemaAsync(this ApplicationDbContext db)
    {
        using var stream = typeof(LoginSchema).Assembly.GetManifestResourceStream("IdentityService.ID-03.sql")
            ?? throw new InvalidOperationException("Missing ID-03 schema resource.");
        using var reader = new StreamReader(stream);
        await db.Database.ExecuteSqlRawAsync(await reader.ReadToEndAsync());
    }
}
