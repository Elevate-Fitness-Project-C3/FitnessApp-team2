namespace TrainingService.Features.Exercises.DeactivateExercise;

using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Common.Results;
using TrainingService.Data;

public class DeactivateExerciseCommandHandler(TrainingDbContext dbContext)
    : IRequestHandler<DeactivateExerciseCommand, Result>
{
    public async Task<Result> Handle(
        DeactivateExerciseCommand command,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        // Direct single-roundtrip database UPDATE via ExecuteUpdateAsync (TR-A01 / AC 02 & AC 03)
        var affectedRows = await dbContext.Exercises
            .Where(e => e.Id == command.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(e => e.IsActive, false)
                .SetProperty(e => e.UpdatedAtUtc, now),
                cancellationToken);

        if (affectedRows == 0)
        {
            return Result.NotFound(
                $"Exercise with ID '{command.Id}' was not found in the catalog.");
        }

        return Result.Success();
    }
}
