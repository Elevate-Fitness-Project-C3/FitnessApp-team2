using IdentityService.Features.Login.ViewModels;

namespace IdentityService.Services.JwtServices;

public interface ITokenService
{
    LoginSessionViewModel GenerateTokens(
        string userId, string email, IEnumerable<string> roles, DateTimeOffset? refreshExpiresAtUtc = null);
}
