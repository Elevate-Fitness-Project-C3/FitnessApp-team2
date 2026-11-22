using FitnessProfileService.Features.CompleteFitnessOnboarding.Orchestrators;
using FluentValidation;

namespace FitnessProfileService.Features.CompleteFitnessOnboarding.Validators;

public class CompleteFitnessOnboardingOrchestratorValidator
    : AbstractValidator<CompleteFitnessOnboardingOrchestrator>
{
    public CompleteFitnessOnboardingOrchestratorValidator()
    {
        

        RuleFor(x => x.Gender)
            .IsInEnum();

        RuleFor(x => x.HeightCm)
            .InclusiveBetween(50, 250);

        RuleFor(x => x.WeightKg)
            .InclusiveBetween(20, 300);

        RuleFor(x => x.PrimaryFitnessGoal)
            .IsInEnum();

        RuleFor(x => x.PhysicalActivityLevel)
            .IsInEnum();

        RuleFor(x => x.DateOfBirth)
            .NotEqual(DateOnly.MinValue)
            .WithMessage("Date of birth is required.")
            .LessThan(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Date of birth must be in the past.");

        RuleFor(x => x.AvailableDaysPerWeek)
            .InclusiveBetween(1, 7);

        RuleFor(x => x.TrainingExperienceLevel)
            .IsInEnum();
    }
}