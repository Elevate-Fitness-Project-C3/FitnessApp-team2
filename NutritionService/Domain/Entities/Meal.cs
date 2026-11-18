using NutritionService.Domain.Enums;

namespace NutritionService.Domain.Entities;

public sealed class Meal
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public MealType MealType { get; private set; }
    public decimal CaloriesKcal { get; private set; }
    public decimal ProteinGrams { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    private Meal() { }

    public static Meal Create(
        string name,
        MealType mealType,
        decimal caloriesKcal,
        decimal proteinGrams)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Meal name must not be empty.", nameof(name));

        if (!Enum.IsDefined(mealType))
            throw new ArgumentException("Invalid meal type.", nameof(mealType));

        if (caloriesKcal < 0)
            throw new ArgumentOutOfRangeException(nameof(caloriesKcal), "Calories must not be negative.");

        if (proteinGrams < 0)
            throw new ArgumentOutOfRangeException(nameof(proteinGrams), "Protein grams must not be negative.");

        return new Meal
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            MealType = mealType,
            CaloriesKcal = caloriesKcal,
            ProteinGrams = proteinGrams,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
    }

    public void Update(
        string name,
        MealType mealType,
        decimal caloriesKcal,
        decimal proteinGrams)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Meal name must not be empty.", nameof(name));

        if (!Enum.IsDefined(mealType))
            throw new ArgumentException("Invalid meal type.", nameof(mealType));

        if (caloriesKcal < 0)
            throw new ArgumentOutOfRangeException(nameof(caloriesKcal), "Calories must not be negative.");

        if (proteinGrams < 0)
            throw new ArgumentOutOfRangeException(nameof(proteinGrams), "Protein grams must not be negative.");

        if (!IsActive)
            throw new InvalidOperationException("Cannot update a deactivated meal.");

        Name = name.Trim();
        MealType = mealType;
        CaloriesKcal = caloriesKcal;
        ProteinGrams = proteinGrams;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("Meal is already deactivated.");

        IsActive = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
