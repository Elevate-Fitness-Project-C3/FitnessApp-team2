using IdentityService.Features.Login.ViewModels;

namespace IdentityService.Services.JwtServices;

public interface ITokenService
{
    Task<LoginSessionViewModel?> CreateSessionAsync(string userId, CancellationToken cancellationToken);
    Task<LoginSessionViewModel?> RefreshSessionAsync(string? refreshToken, CancellationToken cancellationToken);
}
