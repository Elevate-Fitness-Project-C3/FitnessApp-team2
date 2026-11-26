namespace NutritionService.Common.Abstractions;

public interface ISnowflakeIdGenerator
{
    long NextId();
}