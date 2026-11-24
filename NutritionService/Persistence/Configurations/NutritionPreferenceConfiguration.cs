using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutritionService.Domain.Entities;

namespace NutritionService.Persistence.Configurations;

public sealed class NutritionPreferenceConfiguration : IEntityTypeConfiguration<NutritionPreference>
{
    public void Configure(EntityTypeBuilder<NutritionPreference> builder)
    {
    }
}
