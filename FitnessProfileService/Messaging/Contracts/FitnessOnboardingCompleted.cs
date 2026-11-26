using FitnessProfileService.Domain.Enums;

namespace Gym.Contracts
{
    public record FitnessOnboardingCompleted
    (
    Guid UserId,
    FitnessGoal PrimaryFitnessGoal,
    TrainingExperienceLevel TrainingExperienceLevel,
    int AvailableDays
    );
}
