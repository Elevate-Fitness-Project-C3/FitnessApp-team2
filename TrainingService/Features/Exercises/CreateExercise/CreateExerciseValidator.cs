namespace TrainingService.Features.Exercises.CreateExercise;

using FluentValidation;

public class CreateExerciseValidator : AbstractValidator<CreateExerciseCommand>
{
    public CreateExerciseValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Exercise name is required.")
            .MinimumLength(3).WithMessage("Exercise name must be at least 3 characters long.")
            .MaximumLength(150).WithMessage("Exercise name cannot exceed 150 characters.");

        RuleFor(x => x.Instructions)
            .NotEmpty().WithMessage("Exercise instructions are required.")
            .MaximumLength(2000).WithMessage("Instructions cannot exceed 2000 characters.");

        RuleFor(x => x.TargetMuscleGroup)
            .IsInEnum().WithMessage("A valid Target Muscle Group must be selected.");

        RuleFor(x => x.TrackingType)
            .IsInEnum().WithMessage("A valid Tracking Type must be selected.");
    }
}
