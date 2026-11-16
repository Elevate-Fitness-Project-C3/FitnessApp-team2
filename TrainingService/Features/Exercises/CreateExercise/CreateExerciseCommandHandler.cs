namespace TrainingService.Features.Exercises.CreateExercise;

using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Data;
using TrainingService.Domain.Entities;

public class CreateExerciseCommandHandler(TrainingDbContext dbContext)
    : IRequestHandler<CreateExerciseCommand, CreateExerciseResult>
{
    public async Task<CreateExerciseResult> Handle(
        CreateExerciseCommand command,
        CancellationToken cancellationToken)
    {
        // 1. Enforce case-insensitive name uniqueness
        var trimmedName = command.Name.Trim();
        var normalizedName = trimmedName.ToUpper();

        var exists = await dbContext.Exercises
            .AnyAsync(e => e.Name.ToUpper() == normalizedName, cancellationToken);

        if (exists)
        {
            return CreateExerciseResult.Conflict(
                $"An exercise with the name '{trimmedName}' already exists in the catalog.");
        }

        // 2. Create domain entity via factory method
        var exercise = Exercise.Create(
            command.Name,
            command.Instructions,
            command.TargetMuscleGroup,
            command.TrackingType);

        // 3. Persist to isolated Training DB
        dbContext.Exercises.Add(exercise);
        await dbContext.SaveChangesAsync(cancellationToken);

        // 4. Map to Response DTO
        var response = new ExerciseResponse(
            exercise.Id,
            exercise.Name,
            exercise.Instructions,
            exercise.TargetMuscleGroup.ToString(),
            exercise.TrackingType.ToString(),
            exercise.IsActive,
            exercise.CreatedAtUtc,
            exercise.UpdatedAtUtc);

        return CreateExerciseResult.Success(response);
    }
}
