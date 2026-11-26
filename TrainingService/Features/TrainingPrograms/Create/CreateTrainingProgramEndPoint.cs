using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrainingService.Features.TrainingPrograms.DTOs;
using TrainingService.Features.TrainingPrograms.Get;
using TrainingService.Features.TrainingPrograms.PublishProgram;
using TrainingService.TrainingProgram.Commands;
using TrainingService.Validation;

namespace TrainingService.TrainingProgram.EndPoints
{
    public static class CreateTrainingProgramEndPoint
    {
        public static void MapTrainingProgramEndpoints(this IEndpointRouteBuilder app)
        {
            var groupV1 = app.MapGroup("/api/v1/training-programs")
                .WithTags("TrainingPrograms");

            var groupLegacy = app.MapGroup("/api/trainingPrograms")
                .WithTags("TrainingPrograms");

            // Register routes for both /api/v1/training-programs and /api/trainingPrograms
            MapRoutes(groupV1);
            MapRoutes(groupLegacy);
        }

        private static void MapRoutes(RouteGroupBuilder group)
        {
            // POST - Create Training Program (Admin only, starts as Draft)
            group.MapPost("", async ([FromBody] CreateTrainingProgramCommand command, IMediator mediator) =>
            {
                try
                {
                    var trainingProgramId = await mediator.Send(command);
                    return Results.Created($"/api/v1/training-programs/{trainingProgramId}", trainingProgramId);
                }
                catch (RequestValidationException exception)
                {
                    return Results.ValidationProblem(exception.Errors);
                }
            })
            .WithName($"CreateTrainingProgram_{group.GetHashCode()}")
            .RequireAuthorization("AdminOnly")
            .Produces<int>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

            // PATCH /{id}/publish - Publish Training Program (Admin only)
            group.MapPatch("{id:int}/publish", async (int id, IMediator mediator) =>
            {
                try
                {
                    await mediator.Send(new PublishTrainingProgramCommand(id));
                    return Results.NoContent();
                }
                catch (KeyNotFoundException exception)
                {
                    return Results.NotFound(new ProblemDetails
                    {
                        Title = "Not Found",
                        Detail = exception.Message,
                        Status = StatusCodes.Status404NotFound
                    });
                }
            })
            .WithName($"PublishTrainingProgram_{group.GetHashCode()}")
            .RequireAuthorization("AdminOnly")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

            // GET - Browse Published Training Programs Catalog (Public / AllowAnonymous, TR-U01 / AC 01 & AC 04)
            group.MapGet("", async (
                [FromQuery] string? goal,
                [FromQuery] string? level,
                [FromQuery] string? accessTier,
                [FromQuery] string? tier,
                [FromQuery] string? search,
                HttpContext httpContext,
                IMediator mediator) =>
            {
                var hasPremiumAccess = httpContext.User.IsInRole("Premium") ||
                                       httpContext.User.IsInRole("Admin") ||
                                       httpContext.User.HasClaim(c => (c.Type == "roles" || c.Type == "role" || c.Type == System.Security.Claims.ClaimTypes.Role) && (c.Value == "Premium" || c.Value == "Admin"));
                var programs = await mediator.Send(new GetPublishedProgramsQuery(
                    goal,
                    level,
                    accessTier ?? tier,
                    search,
                    hasPremiumAccess));

                return Results.Ok(programs);
            })
            .WithName($"GetPublishedTrainingPrograms_{group.GetHashCode()}")
            .AllowAnonymous()
            .Produces<List<PublishedProgramDTO>>(StatusCodes.Status200OK);

            // GET /{id} - View Published Training Program Details and Ordered Schedule (Public / AllowAnonymous, TR-U01 / AC 02 & AC 03)
            group.MapGet("{id:int}", async (int id, HttpContext httpContext, IMediator mediator) =>
            {
                var hasPremiumAccess = httpContext.User.IsInRole("Premium") ||
                                       httpContext.User.IsInRole("Admin") ||
                                       httpContext.User.HasClaim(c => (c.Type == "roles" || c.Type == "role" || c.Type == System.Security.Claims.ClaimTypes.Role) && (c.Value == "Premium" || c.Value == "Admin"));
                var program = await mediator.Send(new GetPublishedProgramByIdQuery(id, hasPremiumAccess));

                if (program == null)
                {
                    return Results.NotFound(new ProblemDetails
                    {
                        Title = "Not Found",
                        Detail = $"Published training program with ID '{id}' was not found.",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                return Results.Ok(program);
            })
            .WithName($"GetPublishedTrainingProgramById_{group.GetHashCode()}")
            .AllowAnonymous()
            .Produces<PublishedProgramDetailsDTO>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
        }

        // Retained for Program.cs compatibility
        public static void MapCreateTrainingProgramEndPoint(this IEndpointRouteBuilder app)
        {
            app.MapTrainingProgramEndpoints();
        }
    }
}
