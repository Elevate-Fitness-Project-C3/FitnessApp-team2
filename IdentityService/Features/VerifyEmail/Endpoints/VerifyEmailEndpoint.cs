using IdentityService.Features.VerifyEmail.Commands;
using MediatR;

namespace IdentityService.Features.VerifyEmail.Endpoints;

public static class VerifyEmailEndpoint
{
    public static IEndpointRouteBuilder MapVerifyEmailEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/identity/verify-email", async (
            VerifyEmailCommand command, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return TypedResults.Json(result, statusCode: result.StatusCode);
        }).WithTags("Identity").AllowAnonymous();
        return endpoints;
    }
}
