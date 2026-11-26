using FluentValidation;

namespace NutritionService.Features.NutritionGoals.SetNutritionGoal;

public sealed class SetNutritionGoalValidator
    : AbstractValidator<SetNutritionGoalCommand>
{
    public SetNutritionGoalValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(x => x.NutritionGoal)
            .NotNull()
            .WithMessage("Nutrition goal is required.")
            .Must(goal =>
                goal.HasValue &&
                Enum.IsDefined(goal.Value))
            .WithMessage("The specified nutrition goal is invalid.");
    }
}