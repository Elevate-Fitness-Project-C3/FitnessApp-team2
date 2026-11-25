namespace IdentityService.Domain.Entities;

public sealed class EmailVerificationCode
{
    public string UserId { get; set; } = "";
    public ApplicationUser User { get; set; } = null!;
    public required string CodeHash { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public int AttemptCount { get; set; }
}
