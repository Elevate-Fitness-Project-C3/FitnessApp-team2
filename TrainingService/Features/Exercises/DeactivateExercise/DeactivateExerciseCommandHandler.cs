namespace TrainingService.Features.Exercises.DeactivateExercise;

using MediatR;
using TrainingService.Common.Results;
using TrainingService.Domain.Entities;
using TrainingService.Repositories.Interfaces;

public class DeactivateExerciseCommandHandler(
    IGenericRepository<Exercise> exerciseRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeactivateExerciseCommand, Result>
{
    public async Task<Result> Handle(
        DeactivateExerciseCommand command,
        CancellationToken cancellationToken)
    {
        // 1. Fetch tracked entity via Repository
        var exercise = await exerciseRepository.GetByIdAsync(command.Id, cancellationToken);
        if (exercise is null)
        {
            return Result.NotFound(
                $"Exercise with ID '{command.Id}' was not found in the catalog.");
        }

        // 2. Execute rich domain method (preserves change tracking & domain invariants)
        exercise.Deactivate();

        // 3. Persist via Unit of Work with ExecuteAsync
        await unitOfWork.ExecuteAsync(async () =>
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        });

        return Result.Success();
    }
}
