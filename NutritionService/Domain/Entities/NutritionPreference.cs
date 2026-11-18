using NutritionService.Domain.Enums;

namespace NutritionService.Domain.Entities;

public sealed class NutritionPreference
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public NutritionGoal NutritionGoal { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    private NutritionPreference() { }

    public static NutritionPreference Create(Guid userId, NutritionGoal nutritionGoal)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId must not be empty.", nameof(userId));

        if (!Enum.IsDefined(nutritionGoal))
            throw new ArgumentException("Invalid nutrition goal.", nameof(nutritionGoal));

        return new NutritionPreference
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            NutritionGoal = nutritionGoal,
            UpdatedAtUtc = DateTime.UtcNow
        };
    }

    public void ChangeGoal(NutritionGoal newGoal)
    {
        if (!Enum.IsDefined(newGoal))
            throw new ArgumentException("Invalid nutrition goal.", nameof(newGoal));

        NutritionGoal = newGoal;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
