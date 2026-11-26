namespace TrainingService.Features.Exercises.UpdateExercise;

using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Common.Results;
using TrainingService.Domain.Entities;
using TrainingService.Repositories.Interfaces;

public class UpdateExerciseCommandHandler(
    IGenericRepository<Exercise> exerciseRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateExerciseCommand, Result>
{
    public async Task<Result> Handle(
        UpdateExerciseCommand command,
        CancellationToken cancellationToken)
    {
        var trimmedName = command.Name.Trim();
        var normalizedName = trimmedName.ToUpper();

        // 1. Check for name conflict with other exercises (TR-A01 / AC 01 & AC 02)
        var existingExercise = await exerciseRepository.GetAll()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                e => e.Name.ToUpper() == normalizedName && e.Id != command.Id,
                cancellationToken);

        if (existingExercise is not null)
        {
            return Result.Conflict(
                $"An exercise with the name '{trimmedName}' already exists in the catalog.");
        }

        // 2. Fetch tracked entity via Repository
        var exercise = await exerciseRepository.GetByIdAsync(command.Id, cancellationToken);
        if (exercise is null)
        {
            return Result.NotFound(
                $"Exercise with ID '{command.Id}' was not found in the catalog.");
        }

        // 3. Update domain state via rich domain method (preserves domain invariants & change tracking)
        exercise.Update(
            command.Name,
            command.Instructions,
            command.TargetMuscleGroup,
            command.TrackingType);

        // 4. Persist changes consistently via Unit of Work with ExecuteAsync
        await unitOfWork.ExecuteAsync(async () =>
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        });

        return Result.Success();
    }
}
