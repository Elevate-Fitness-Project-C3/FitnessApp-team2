namespace IdentityService.Domain.Entities;

public class RefreshSession
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = default!;
    public Guid JwtId { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? RevokedAtUtc { get; private set; }
    public Guid? ReplacedBySessionId { get; private set; }

    private static readonly TimeSpan SessionLifetime = TimeSpan.FromDays(7);

    private RefreshSession() { }

    public static RefreshSession Create(Guid userId, string tokenHash, Guid jwtId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID must not be empty.", nameof(userId));

        ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);

        if (jwtId == Guid.Empty)
            throw new ArgumentException("JWT ID must not be empty.", nameof(jwtId));

        var now = DateTime.UtcNow;

        return new RefreshSession
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            JwtId = jwtId,
            IsRevoked = false,
            ExpiresAtUtc = now.Add(SessionLifetime),
            CreatedAtUtc = now,
            RevokedAtUtc = null,
            ReplacedBySessionId = null
        };
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow >= ExpiresAtUtc;
    }

    public bool IsActive()
    {
        return !IsRevoked && !IsExpired();
    }

    public void Revoke(Guid? replacedBySessionId = null)
    {
        if (IsRevoked)
            throw new InvalidOperationException("Session is already revoked.");

        IsRevoked = true;
        RevokedAtUtc = DateTime.UtcNow;
        ReplacedBySessionId = replacedBySessionId;
    }
}
