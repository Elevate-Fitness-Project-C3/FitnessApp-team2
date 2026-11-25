using FitnessProfileService.Domain.Enums;

namespace FitnessProfileService.Features.CompleteFitnessOnboarding.DTOs
{
    public record OnboardingEligibilityDto (bool ProfileExists, OnboardingStatus? OnboardingStatus);
 
}
