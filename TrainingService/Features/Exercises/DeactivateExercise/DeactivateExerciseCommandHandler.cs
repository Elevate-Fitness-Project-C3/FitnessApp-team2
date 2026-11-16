namespace TrainingService.Features.Exercises.DeactivateExercise;

using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Data;

public class DeactivateExerciseCommandHandler(TrainingDbContext dbContext)
    : IRequestHandler<DeactivateExerciseCommand, DeactivateExerciseResult>
{
    public async Task<DeactivateExerciseResult> Handle(
        DeactivateExerciseCommand command,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        // Direct single-roundtrip database UPDATE via ExecuteUpdateAsync
        var affectedRows = await dbContext.Exercises
            .Where(e => e.Id == command.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(e => e.IsActive, false)
                .SetProperty(e => e.UpdatedAtUtc, now),
                cancellationToken);

        if (affectedRows == 0)
        {
            return DeactivateExerciseResult.NotFound(
                $"Exercise with ID '{command.Id}' was not found in the catalog.");
        }

        return DeactivateExerciseResult.Success();
    }
}
