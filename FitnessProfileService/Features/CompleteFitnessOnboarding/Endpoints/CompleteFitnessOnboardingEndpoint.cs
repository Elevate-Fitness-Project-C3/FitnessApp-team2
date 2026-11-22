using FitnessProfileService.Common.Responses;
using FitnessProfileService.Features.CompleteFitnessOnboarding.Orchestrators;
using FitnessProfileService.Features.CompleteFitnessOnboarding.ViewModels;
using MediatR;
using System.Security.Claims;

namespace FitnessProfileService.Features.CompleteFitnessOnboarding.Endpoints
{
    public static class CompleteFitnessOnboardingEndpoint
    {
        public static void MapPostCancelEnrollmentEndpoint(this IEndpointRouteBuilder app)
        {
           
            var group = app.MapGroup("/api/fitness-profile")
                //.RequireAuthorization()
                .WithTags("Fitness Profile");

            group.MapPost("/onboarding/complete",
                async (
                    CompleteFitnessOnboardingViewModel request,
                    //ClaimsPrincipal user,
                    IMediator mediator,
                    CancellationToken cancellationToken) =>
                {
                    //var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

                    //if (!Guid.TryParse(userIdClaim, out var userId))
                    //    return Results.Unauthorized();

                    var command = new CompleteFitnessOnboardingOrchestrator
                    (
                        UserId :Guid.NewGuid(),
                        Gender : request.Gender,
                        DateOfBirth : request.DateOfBirth,
                        HeightCm : request.HeightCm,
                        WeightKg : request.WeightKg,
                        PrimaryFitnessGoal : request.PrimaryFitnessGoal,
                        PhysicalActivityLevel : request.PhysicalActivityLevel,
                        AvailableDaysPerWeek : request.AvailableDaysPerWeek,
                        TrainingExperienceLevel : request.TrainingExperienceLevel
                    );

                    var res = await mediator.Send(command, cancellationToken);

                    return Results.Json<EndpointResponse<bool>>(EndpointResponse<bool>.FromResult(res),statusCode:res.StatusCode);
                //: Results.Json(EndpointResponse.Fail(res.Message, res.StatusCode), statusCode: res.StatusCode);
                })
                .WithName("CompleteFitnessOnboarding");
        }
    }
    }

