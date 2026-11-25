using IdGen;
using NutritionService.Common.Abstractions;

namespace NutritionService.Infrastructure.Identifiers;

public sealed class SnowflakeIdGenerator : ISnowflakeIdGenerator
{
    private readonly IdGenerator _generator;

    public SnowflakeIdGenerator(int workerId)
    {
        _generator = new IdGenerator(workerId);
    }

    public long NextId()
    {
        return _generator.CreateId();
    }
}