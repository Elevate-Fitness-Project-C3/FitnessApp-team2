using NutritionService.Domain.Enums;

namespace NutritionService.Features.NutritionGoals.SetNutritionGoal;

public sealed record SetNutritionGoalRequest(
    NutritionGoal? NutritionGoal);