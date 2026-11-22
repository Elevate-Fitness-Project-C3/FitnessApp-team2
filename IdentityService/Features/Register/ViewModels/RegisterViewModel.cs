namespace IdentityService.Features.Register.ViewModels;

public sealed record RegisterViewModel(
    string UserId,
    string Email,
    bool EmailConfirmed,
    string VerificationStatus);
