namespace TrainingService.Features.Exercises.CreateExercise;

using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Common.Results;
using TrainingService.Domain.Entities;
using TrainingService.Repositories.Interfaces;

public class CreateExerciseCommandHandler(
    IGenericRepository<Exercise> exerciseRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateExerciseCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        CreateExerciseCommand command,
        CancellationToken cancellationToken)
    {
        // 1. Enforce case-insensitive name uniqueness (TR-A01 / AC 01)
        var trimmedName = command.Name.Trim();
        var normalizedName = trimmedName.ToUpper();

        var existingExercise = await exerciseRepository.GetAll()
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Name.ToUpper() == normalizedName, cancellationToken);

        if (existingExercise is not null)
        {
            return Result<Guid>.Conflict(
                $"An exercise with the name '{trimmedName}' already exists in the catalog.");
        }

        // 2. Create domain entity via factory method
        var exercise = Exercise.Create(
            command.Name,
            command.Instructions,
            command.TargetMuscleGroup,
            command.TrackingType);

        // 3. Persist via Unit of Work with ExecuteAsync
        await unitOfWork.ExecuteAsync(async () =>
        {
            exerciseRepository.Add(exercise);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        });

        // 4. Return new exercise ID per CQRS conventions
        return Result<Guid>.Success(exercise.Id);
    }
}
