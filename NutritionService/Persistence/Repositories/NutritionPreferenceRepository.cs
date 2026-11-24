using Microsoft.EntityFrameworkCore;
using NutritionService.Domain.Entities;
using NutritionService.Domain.Repositories;

namespace NutritionService.Persistence.Repositories;

public sealed class NutritionPreferenceRepository
    : INutritionPreferenceRepository
{
    private readonly NutritionDbContext _context;

    public NutritionPreferenceRepository(
        NutritionDbContext context)
    {
        _context = context;
    }

    public Task<NutritionPreference?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return _context.NutritionPreferences
            .SingleOrDefaultAsync(
                x => x.UserId == userId,
                cancellationToken);
    }

    public void Add(NutritionPreference preference)
    {
        _context.NutritionPreferences.Add(preference);
    }
}