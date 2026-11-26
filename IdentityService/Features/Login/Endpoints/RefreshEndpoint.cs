using IdentityService.Common.ResultPattern;
using IdentityService.Features.Login.Commands;
using IdentityService.Features.Login.DTOs;
using MediatR;

namespace IdentityService.Features.Login.Endpoints;

public static class RefreshEndpoint
{
    public static IEndpointRouteBuilder MapRefreshEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/identity/refresh", async (
            RefreshSessionCommand command, IMediator mediator, HttpContext context, CancellationToken cancellationToken) =>
        {
            context.Response.Headers.CacheControl = "no-store";
            var result = await mediator.Send(command, cancellationToken);
            return TypedResults.Json(result, statusCode: result.StatusCode);
        })
        .WithName("RefreshSession")
        .WithTags("Identity")
        .AllowAnonymous()
        .Produces<RequestResponse<LoginSessionDto>>()
        .Produces<RequestResponse<LoginSessionDto>>(StatusCodes.Status401Unauthorized);
        return endpoints;
    }
}
