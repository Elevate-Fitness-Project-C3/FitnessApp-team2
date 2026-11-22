namespace NutritionService.Domain.Enums;

/// <summary>
/// Classifies meals in the catalog by their typical consumption time.
/// UNRESOLVED: These values have not been confirmed by the source business documents
/// (Elevate_Fitness_Backend_User_Stories_v6_With_Design_References.pdf).
/// Validate against approved requirements before finalizing.
/// </summary>
public enum MealType
{
    Breakfast = 0,
    Lunch = 1,
    Dinner = 2,
    Snack = 3
}
