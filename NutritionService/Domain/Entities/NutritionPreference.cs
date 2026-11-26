using NutritionService.Common;
using NutritionService.Domain.Enums;
using NutritionService.Domain.Errors;

namespace NutritionService.Domain.Entities;

public sealed class NutritionPreference
{
    public long Id { get; private set; }

    public Guid UserId { get; private set; }

    public NutritionGoal NutritionGoal { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    private NutritionPreference()
    {
    }

    private NutritionPreference(
        long id,
        Guid userId,
        NutritionGoal nutritionGoal)
    {
        Id = id;
        UserId = userId;
        NutritionGoal = nutritionGoal;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public static Result<NutritionPreference> Create(
        long id,
        Guid userId,
        NutritionGoal nutritionGoal)
    {
        if (id <= 0)
        {
            return Result.Failure<NutritionPreference>(
                NutritionPreferenceErrors.InvalidId);
        }

        if (userId == Guid.Empty)
        {
            return Result.Failure<NutritionPreference>(
                NutritionPreferenceErrors.EmptyUserId);
        }

        if (!Enum.IsDefined(nutritionGoal))
        {
            return Result.Failure<NutritionPreference>(
                NutritionPreferenceErrors.InvalidGoal);
        }

        var preference = new NutritionPreference(
            id,
            userId,
            nutritionGoal);

        return Result.Success(preference);
    }

    public Result ChangeGoal(NutritionGoal nutritionGoal)
    {
        if (!Enum.IsDefined(nutritionGoal))
        {
            return Result.Failure(
                NutritionPreferenceErrors.InvalidGoal);
        }

        NutritionGoal = nutritionGoal;
        UpdatedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }
}