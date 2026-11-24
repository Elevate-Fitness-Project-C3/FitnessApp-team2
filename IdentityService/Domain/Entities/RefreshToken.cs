using System.Security.Cryptography;
using System.Text;

namespace IdentityService.Domain.Entities;

public sealed class RefreshToken
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string UserId { get; init; }
    public ApplicationUser User { get; init; } = null!;
    public required string TokenHash { get; set; }
    public required DateTimeOffset ExpiresAtUtc { get; init; }

}
