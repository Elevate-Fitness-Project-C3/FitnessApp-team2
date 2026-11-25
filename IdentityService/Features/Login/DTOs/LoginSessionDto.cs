namespace IdentityService.Features.Login.DTOs;

public sealed class LoginSessionDto
{
    public required string AccessToken { get; init; }
    public required DateTimeOffset AccessTokenExpiresAtUtc { get; init; }
    public required string RefreshToken { get; init; }
    public required DateTimeOffset RefreshTokenExpiresAtUtc { get; init; }
}
