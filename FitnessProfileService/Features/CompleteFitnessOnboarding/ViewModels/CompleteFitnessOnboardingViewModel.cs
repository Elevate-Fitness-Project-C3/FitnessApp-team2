using FitnessProfileService.Domain.Enums;

namespace FitnessProfileService.Features.CompleteFitnessOnboarding.ViewModels
{
    public class CompleteFitnessOnboardingViewModel
    {
        public Gender Gender { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public decimal HeightCm { get; set; }

        public decimal WeightKg { get; set; }

        public FitnessGoal PrimaryFitnessGoal { get; set; }

        public PhysicalActivityLevel PhysicalActivityLevel { get; set; }

        public int AvailableDaysPerWeek { get; set; }

        public TrainingExperienceLevel TrainingExperienceLevel { get; set; }
    }
}
