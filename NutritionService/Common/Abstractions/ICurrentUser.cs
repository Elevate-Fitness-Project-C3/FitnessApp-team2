namespace NutritionService.Common.Abstractions;

public interface ICurrentUser
{
    Guid? UserId { get; }
}