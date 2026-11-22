using IdentityService.Common.ResultPattern;
using IdentityService.Features.Login.Commands;
using IdentityService.Features.Login.ViewModels;
using MediatR;

namespace IdentityService.Features.Login.Endpoints;

public static class LoginEndpoint
{
    public static IEndpointRouteBuilder MapLoginEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/identity/login", HandleAsync)
            .WithName("Login")
            .WithTags("Identity")
            .AllowAnonymous()
            .Produces<RequestResponse<LoginSessionViewModel>>()
            .Produces<RequestResponse<LoginSessionViewModel>>(StatusCodes.Status400BadRequest)
            .Produces<RequestResponse<LoginSessionViewModel>>(StatusCodes.Status401Unauthorized);

        return endpoints;
    }

    private static async Task<IResult> HandleAsync(
        LoginCommand command, IMediator mediator, HttpResponse response, CancellationToken cancellationToken)
    {
        response.Headers.CacheControl = "no-store";
        var result = await mediator.Send(command, cancellationToken);
        return TypedResults.Json(result, statusCode: result.StatusCode);
    }
}
