using MediatR;
using NutritionService.Common;
using NutritionService.Domain.Enums;

namespace NutritionService.Features.NutritionGoals.SetNutritionGoal;

public sealed record SetNutritionGoalCommand(
    Guid UserId,
    NutritionGoal? NutritionGoal
) : IRequest<Result>;