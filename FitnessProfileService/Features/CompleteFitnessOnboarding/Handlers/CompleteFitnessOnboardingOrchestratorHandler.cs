using FitnessProfileService.Common.Responses;
using FitnessProfileService.Domain.Contracts;
using FitnessProfileService.Domain.Enums;
using FitnessProfileService.Features.CompleteFitnessOnboarding.Commands;
using FitnessProfileService.Features.CompleteFitnessOnboarding.Orchestrators;
using FitnessProfileService.Features.CompleteFitnessOnboarding.Queries;
using Gym.Contracts;
using MassTransit;
using MediatR;

namespace FitnessProfileService.Features.CompleteFitnessOnboarding.Handlers
{
    public class CompleteFitnessOnboardingOrchestratorHandler : IRequestHandler<CompleteFitnessOnboardingOrchestrator, RequestResponse<bool>>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;

        public CompleteFitnessOnboardingOrchestratorHandler(IMediator mediator,
                                                            IUnitOfWork unitOfWork,
                                                            IPublishEndpoint publishEndpoint)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<RequestResponse<bool>> Handle(CompleteFitnessOnboardingOrchestrator request, CancellationToken cancellationToken)
        {

            //Validate

            // 2. Validate that UserId exists and is a valid Guid.
            //    If not, return Unauthorized.
            if (!Guid.TryParse(request.UserId, out var userId))
                return RequestResponse<bool>.Fail("Invalid user ID.", StatusCodes.Status401Unauthorized);

          

            //db=>> 3. Execute CheckOnboardingEligibilityQuery using UserId.
            var onboardingEligibilityResponse = await _mediator.Send(new CheckOnboardingEligibilityQuery(userId), cancellationToken);
            if (!onboardingEligibilityResponse.Success ||
                onboardingEligibilityResponse.Data is null)
            {
                return RequestResponse<bool>.Fail(
                    "Failed to check onboarding eligibility.",
                    StatusCodes.Status500InternalServerError);
            }

            // 4. Check if FitnessProfile exists.
            if (!onboardingEligibilityResponse.Data.ProfileExists)
            {
                return RequestResponse<bool>.Fail(
                    "Fitness profile not found.",
                    StatusCodes.Status404NotFound);
            }
            // 5. Check OnboardingStatus:
            //    - NotStarted: Allow onboarding.
            //    - InProgress: Allow retry/resuming onboarding.
            //    - Completed: Reject the request with Conflict.
            if (onboardingEligibilityResponse.Data.OnboardingStatus == OnboardingStatus.Completed)
            {
                return RequestResponse<bool>.Fail(
                      "Invalid onboarding status.",
                      StatusCodes.Status409Conflict);
            }

            // 6. Execute onboarding command.
            var commandResponse = await _mediator.Send(
                new CompleteFitnessOnboardingCommand(
                    userId,
                    request.Gender,
                    request.DateOfBirth,
                    request.HeightCm,
                    request.WeightKg,
                    request.PrimaryFitnessGoal,
                    request.PhysicalActivityLevel,
                    request.AvailableDaysPerWeek,
                    request.TrainingExperienceLevel),
                cancellationToken);

            if (!commandResponse.Success)
            {
                return commandResponse;
            }
            await _publishEndpoint.Publish(
                new FitnessOnboardingCompleted(userId,
                                               request.PrimaryFitnessGoal,
                                               request.TrainingExperienceLevel,
                                               request.AvailableDaysPerWeek));

            // 7. Return success.
            return RequestResponse<bool>.Ok(true);
        }
    }
}

