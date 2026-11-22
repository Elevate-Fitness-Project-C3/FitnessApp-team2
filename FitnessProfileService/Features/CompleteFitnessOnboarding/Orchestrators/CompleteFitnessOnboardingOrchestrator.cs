using FitnessProfileService.Common.Responses;
using FitnessProfileService.Domain.Enums;
using MediatR;

namespace FitnessProfileService.Features.CompleteFitnessOnboarding.Orchestrators
{
    public record CompleteFitnessOnboardingOrchestrator(
           Guid UserId,
           Gender Gender,
           decimal HeightCm,
           decimal WeightKg,
           FitnessGoal PrimaryFitnessGoal,
           PhysicalActivityLevel PhysicalActivityLevel,
           DateOnly DateOfBirth,
           int AvailableDaysPerWeek,
            TrainingExperienceLevel TrainingExperienceLevel
           ) : IRequest< RequestResponse< bool>>;
    
    
}
