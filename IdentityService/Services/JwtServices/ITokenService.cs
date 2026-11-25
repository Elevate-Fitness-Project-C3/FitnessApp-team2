using IdentityService.Features.Login.DTOs;

namespace IdentityService.Services.JwtServices;

public interface ITokenService
{
    LoginSessionDto GenerateTokens(
        string userId, string email, IEnumerable<string> roles, DateTimeOffset? refreshExpiresAtUtc = null);
}
