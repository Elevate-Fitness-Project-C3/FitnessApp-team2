namespace TrainingService.Features.Exercises.GetExercises;

using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Data;

public class GetExercisesQueryHandler(TrainingDbContext dbContext)
    : IRequestHandler<GetExercisesQuery, IReadOnlyList<ExerciseResponse>>
{
    public async Task<IReadOnlyList<ExerciseResponse>> Handle(
        GetExercisesQuery query,
        CancellationToken cancellationToken)
    {
        var dbQuery = dbContext.Exercises.AsNoTracking();

        // 1. Filter by Active / Inactive status 
        if (query.IsActive.HasValue)
        {
            dbQuery = dbQuery.Where(e => e.IsActive == query.IsActive.Value);
        }

        // 2. Filter by Target Muscle Group if provided
        if (query.TargetMuscleGroup.HasValue)
        {
            dbQuery = dbQuery.Where(e => e.TargetMuscleGroup == query.TargetMuscleGroup.Value);
        }

        // 3. Filter by search keyword (case-insensitive substring match)
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var trimmedSearch = query.Search.Trim().ToUpper();
            dbQuery = dbQuery.Where(e => e.Name.ToUpper().Contains(trimmedSearch));
        }

        // 4. Project results ordered alphabetically by Name
        var exercises = await dbQuery
            .OrderBy(e => e.Name)
            .Select(e => new ExerciseResponse(
                e.Id,
                e.Name,
                e.Instructions,
                e.TargetMuscleGroup.ToString(),
                e.TrackingType.ToString(),
                e.IsActive,
                e.CreatedAtUtc,
                e.UpdatedAtUtc))
            .ToListAsync(cancellationToken);

        return exercises;
    }
}
