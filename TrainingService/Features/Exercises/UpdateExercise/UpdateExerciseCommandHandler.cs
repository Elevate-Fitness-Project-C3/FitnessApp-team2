namespace TrainingService.Features.Exercises.UpdateExercise;

using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Common.Results;
using TrainingService.Data;

public class UpdateExerciseCommandHandler(TrainingDbContext dbContext)
    : IRequestHandler<UpdateExerciseCommand, Result<ExerciseResponse>>
{
    public async Task<Result<ExerciseResponse>> Handle(
        UpdateExerciseCommand command,
        CancellationToken cancellationToken)
    {
        var trimmedName = command.Name.Trim();
        var normalizedName = trimmedName.ToUpper();

        // 1. Check for name conflict with other exercises (TR-A01 / AC 01 & AC 02)
        var nameConflict = await dbContext.Exercises
            .AnyAsync(e => e.Name.ToUpper() == normalizedName && e.Id != command.Id, cancellationToken);

        if (nameConflict)
        {
            return Result<ExerciseResponse>.Conflict(
                $"An exercise with the name '{trimmedName}' already exists in the catalog.");
        }

        // 2. Direct database UPDATE via ExecuteUpdateAsync
        var now = DateTime.UtcNow;
        var affectedRows = await dbContext.Exercises
            .Where(e => e.Id == command.Id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(e => e.Name, trimmedName)
                .SetProperty(e => e.Instructions, command.Instructions.Trim())
                .SetProperty(e => e.TargetMuscleGroup, command.TargetMuscleGroup)
                .SetProperty(e => e.TrackingType, command.TrackingType)
                .SetProperty(e => e.UpdatedAtUtc, now),
                cancellationToken);

        if (affectedRows == 0)
        {
            return Result<ExerciseResponse>.NotFound(
                $"Exercise with ID '{command.Id}' was not found in the catalog.");
        }

        // 3. Project updated exercise to return to client (AsNoTracking)
        var updatedExercise = await dbContext.Exercises
            .AsNoTracking()
            .Where(e => e.Id == command.Id)
            .Select(e => new ExerciseResponse(
                e.Id,
                e.Name,
                e.Instructions,
                e.TargetMuscleGroup.ToString(),
                e.TrackingType.ToString(),
                e.IsActive,
                e.CreatedAtUtc,
                e.UpdatedAtUtc))
            .FirstOrDefaultAsync(cancellationToken);

        return Result<ExerciseResponse>.Success(updatedExercise!);
    }
}
