using NutritionService.Common;

namespace NutritionService.Domain.Errors;

public static class NutritionPreferenceErrors
{
    public static readonly Error EmptyUserId = new(
        "NutritionPreference.EmptyUserId",
        "User ID cannot be empty.");

    public static readonly Error InvalidGoal = new(
        "NutritionPreference.InvalidGoal",
        "Nutrition goal is invalid.");

    public static readonly Error InvalidId = new(
        "NutritionPreference.InvalidId",
        "Nutrition preference ID must be greater than zero.");

    public static readonly Error NotFound = new(
        "NutritionPreference.NotFound",
        "No nutrition preference has been saved for this user.");
}