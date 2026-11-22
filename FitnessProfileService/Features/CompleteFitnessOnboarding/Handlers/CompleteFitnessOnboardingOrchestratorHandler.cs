using FitnessProfileService.Common.Responses;
using FitnessProfileService.Domain.Contracts;
using FitnessProfileService.Domain.Enums;
using FitnessProfileService.Features.CompleteFitnessOnboarding.Commands;
using FitnessProfileService.Features.CompleteFitnessOnboarding.Orchestrators;
using FitnessProfileService.Features.CompleteFitnessOnboarding.Queries;
using MediatR;

namespace FitnessProfileService.Features.CompleteFitnessOnboarding.Handlers
{
    public class CompleteFitnessOnboardingOrchestratorHandler : IRequestHandler<CompleteFitnessOnboardingOrchestrator, RequestResponse<bool>>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public CompleteFitnessOnboardingOrchestratorHandler(IMediator mediator,IUnitOfWork unitOfWork)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        public async Task<RequestResponse<bool>> Handle(CompleteFitnessOnboardingOrchestrator request, CancellationToken cancellationToken)
        {
            
            //Validate

            // 1. Get UserId from authenticated user's claims.

            // 2. Validate that UserId exists and is a valid Guid.
            //    If not, return Unauthorized.

            //db=>> 3. Execute CheckOnboardingEligibilityQuery using UserId.
            var onboardingEligibilityResponse = await _mediator.Send(new CheckOnboardingEligibilityQuery(request.UserId), cancellationToken);
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
                    request.UserId,
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

            // 7. Return success.
            return RequestResponse<bool>.Ok(true);
        }
    }
}

