using NutritionService.Common.Abstractions;

namespace NutritionService.Infrastructure.Authentication;

/// <summary>
/// Extracts the authenticated user's identity from the validated JWT claims.
/// UserId is sourced exclusively from the "sub" claim — never from the request body.
/// </summary>
public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;

            var subValue = user?.FindFirst("sub")?.Value;

            return Guid.TryParse(subValue, out var userId)
                ? userId
                : null;
        }
    }
}
