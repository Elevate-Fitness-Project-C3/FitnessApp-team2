using NutritionService.Domain.Entities;

namespace NutritionService.Domain.Repositories;

public interface INutritionPreferenceRepository
{
    Task<NutritionPreference?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken);

    void Add(NutritionPreference preference);
}