using System.Security.Cryptography;
using System.Text;

namespace IdentityService.Domain.Entities;

public sealed class RefreshSession
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string UserId { get; init; }
    public ApplicationUser User { get; init; } = null!;
    public required string TokenHash { get; set; }
    public required DateTimeOffset ExpiresAtUtc { get; init; }

    public static string HashToken(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
