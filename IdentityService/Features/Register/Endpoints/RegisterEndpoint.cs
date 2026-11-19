using IdentityService.Common.ResultPattern;
using IdentityService.Features.Register.Commands;
using IdentityService.Features.Register.ViewModels;
using MediatR;

namespace IdentityService.Features.Register.Endpoints;

public static class RegisterEndpoint
{
    public static IEndpointRouteBuilder MapRegisterEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/identity/register", HandleAsync)
            .WithName("Register")
            .WithTags("Identity")
            .AllowAnonymous()
            .RequireRateLimiting("registration")
            .Produces<RequestResponse<RegisterViewModel>>(StatusCodes.Status201Created)
            .Produces<RequestResponse<RegisterViewModel>>(StatusCodes.Status400BadRequest)
            .Produces<RequestResponse<RegisterViewModel>>(StatusCodes.Status409Conflict)
            .Produces<RequestResponse<RegisterViewModel>>(StatusCodes.Status503ServiceUnavailable)
            .Produces<RequestResponse<object>>(StatusCodes.Status429TooManyRequests);
        return endpoints;
    }

    private static async Task<IResult> HandleAsync(
        RegisterCommand command, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return TypedResults.Json(result, statusCode: result.StatusCode);
    }
}
