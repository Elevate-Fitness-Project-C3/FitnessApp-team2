namespace IdentityService.Features.Login.DTOs;

public sealed record AuthenticatedUserDto(string UserId, string Email, IList<string> Roles);
