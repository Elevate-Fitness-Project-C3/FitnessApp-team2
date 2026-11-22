using FitnessProfileService.Common.Responses;
using FitnessProfileService.Features.CompleteFitnessOnboarding.DTOs;
using MediatR;

namespace FitnessProfileService.Features.CompleteFitnessOnboarding.Queries
{
    public record CheckOnboardingEligibilityQuery(Guid UserId)
                    : IRequest<RequestResponse<OnboardingEligibilityDto>>;
}
