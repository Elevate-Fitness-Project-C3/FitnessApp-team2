namespace TrainingService.Features.Exercises.GetExerciseById;

using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Data;

public class GetExerciseByIdQueryHandler(TrainingDbContext dbContext)
    : IRequestHandler<GetExerciseByIdQuery, ExerciseResponse?>
{
    public async Task<ExerciseResponse?> Handle(
        GetExerciseByIdQuery query,
        CancellationToken cancellationToken)
    {
        var exercise = await dbContext.Exercises
            .AsNoTracking()
            .Where(e => e.Id == query.Id)
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

        return exercise;
    }
}
