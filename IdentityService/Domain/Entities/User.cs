namespace IdentityService.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = default!;
    public string NormalizedEmail { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string Role { get; private set; } = default!;
    public bool IsEmailVerified { get; private set; }
    public bool IsEnabled { get; private set; }
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockoutEndUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    private const int MaxFailedLoginAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);
    private const string DefaultRole = "User";

    private User() { }

    public static User Create(string email, string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);

        var now = DateTime.UtcNow;

        return new User
        {
            Id = Guid.NewGuid(),
            Email = email.Trim(),
            NormalizedEmail = email.Trim().ToUpperInvariant(),
            PasswordHash = passwordHash,
            Role = DefaultRole,
            IsEmailVerified = false,
            IsEnabled = true,
            FailedLoginAttempts = 0,
            LockoutEndUtc = null,
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };
    }

    public bool IsLockedOut()
    {
        return LockoutEndUtc.HasValue && DateTime.UtcNow < LockoutEndUtc.Value;
    }

    public void RecordFailedLogin()
    {
        if (IsLockedOut())
            return;

        FailedLoginAttempts++;
        UpdatedAtUtc = DateTime.UtcNow;

        if (FailedLoginAttempts >= MaxFailedLoginAttempts)
        {
            LockoutEndUtc = DateTime.UtcNow.Add(LockoutDuration);
        }
    }

    public void RecordSuccessfulLogin()
    {
        FailedLoginAttempts = 0;
        LockoutEndUtc = null;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void VerifyEmail()
    {
        if (IsEmailVerified)
            throw new InvalidOperationException("Email is already verified.");

        IsEmailVerified = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void ChangePasswordHash(string newPasswordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newPasswordHash);

        PasswordHash = newPasswordHash;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Disable()
    {
        if (!IsEnabled)
            throw new InvalidOperationException("Account is already disabled.");

        IsEnabled = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Enable()
    {
        if (IsEnabled)
            throw new InvalidOperationException("Account is already enabled.");

        IsEnabled = true;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
