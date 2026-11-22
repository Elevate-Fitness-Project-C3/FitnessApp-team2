using FitnessProfileService.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FitnessProfileService.Domain.Entities
{
    public class FitnessProfile 
    {
        public Guid UserId { get; set; } = default!;
        public Gender? Gender { get; set; }
        public OnboardingStatus OnboardingStatus { get; set; }= OnboardingStatus.NotStarted;
        public DateOnly? DateOfBirth { get; set; }
        public FitnessGoal? PrimaryFitnessGoal { get; set; }
        public PhysicalActivityLevel? ActivityLevel { get; set; }
        public TrainingExperienceLevel? TrainingExperienceLevel { get; set; }
        [Range(1,7)]
        public int? AvailableDaysPerWeek { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset? UpdatedAt { get; set; }

        // Navigation Property
        public ICollection<BodyMeasurement> BodyMeasurements { get; set; } = [];
     


        //derived
        public int? Age
        {
            get
            {
                if (!DateOfBirth.HasValue)
                {
                    return null;
                }
                var today = DateOnly.FromDateTime(DateTime.Today);

                var age = today.Year - DateOfBirth.Value.Year;

                if (DateOfBirth > today.AddYears(-age))
                    age--;
                return age;
            }
        }

    }
}
