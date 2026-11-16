using IdentityService.Domain.Enums;

namespace IdentityService.Domain.Entities;

public class VerificationChallenge
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string OtpHash { get; private set; } = default!;
    public ChallengePurpose Purpose { get; private set; }
    public int FailedAttempts { get; private set; }
    public bool IsConsumed { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private const int MaxAttempts = 5;
    private static readonly TimeSpan ExpirationDuration = TimeSpan.FromMinutes(10);

    private VerificationChallenge() { }

    public static VerificationChallenge Create(Guid userId, string otpHash, ChallengePurpose purpose)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID must not be empty.", nameof(userId));

        ArgumentException.ThrowIfNullOrWhiteSpace(otpHash);

        var now = DateTime.UtcNow;

        return new VerificationChallenge
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            OtpHash = otpHash,
            Purpose = purpose,
            FailedAttempts = 0,
            IsConsumed = false,
            ExpiresAtUtc = now.Add(ExpirationDuration),
            CreatedAtUtc = now
        };
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow >= ExpiresAtUtc;
    }

    public bool IsAttemptsExhausted()
    {
        return FailedAttempts >= MaxAttempts;
    }

    public bool IsUsable()
    {
        return !IsConsumed && !IsExpired() && !IsAttemptsExhausted();
    }

    public void RecordFailedAttempt()
    {
        if (!IsUsable())
            throw new InvalidOperationException("Challenge is no longer usable.");

        FailedAttempts++;
    }

    public void Consume()
    {
        if (!IsUsable())
            throw new InvalidOperationException("Challenge is no longer usable.");

        IsConsumed = true;
    }
}
