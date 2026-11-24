using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Repositories.Interfaces;
using TrainingService.Workout.Entities;

namespace TrainingService.Features.Workouts.Get
{
    public record GetWorkoutsQuery(string? Difficulty = null, string? Search = null)
        : IRequest<List<GetWorkoutByIdQueryDTO>>;

    public class GetWorkoutsQueryHandler
        : IRequestHandler<GetWorkoutsQuery, List<GetWorkoutByIdQueryDTO>>
    {
        private readonly IGenericRepository<Workout.Entities.Workout> _repository;

        public GetWorkoutsQueryHandler(
            IGenericRepository<Workout.Entities.Workout> repository)
        {
            _repository = repository;
        }

        public async Task<List<GetWorkoutByIdQueryDTO>> Handle(
            GetWorkoutsQuery request,
            CancellationToken cancellationToken)
        {
            var query = _repository.GetAll()
                .Include(w => w.WorkoutExercises)
                    .ThenInclude(we => we.Exercise)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Difficulty))
            {
                var diff = request.Difficulty.Trim();
                query = query.Where(w => w.Difficulty == diff);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim();
                query = query.Where(w => w.Title.Contains(search) || w.Description.Contains(search));
            }

            var workouts = await query.ToListAsync(cancellationToken);

            return workouts.Select(w => new GetWorkoutByIdQueryDTO
            {
                Id = w.Id,
                Title = w.Title,
                Description = w.Description,
                Difficulty = w.Difficulty,
                EstimatedDurationMinutes = w.EstimatedDurationMinutes,
                Exercises = w.WorkoutExercises
                    .OrderBy(we => we.OrderIndex)
                    .Select(we => new WorkoutExerciseDetailsDTO
                    {
                        Id = we.Id,
                        ExerciseId = we.ExerciseId,
                        ExerciseName = we.Exercise != null ? we.Exercise.Name : string.Empty,
                        OrderIndex = we.OrderIndex,
                        Sets = we.Sets,
                        Reps = we.Reps,
                        DurationSeconds = we.DurationSeconds,
                        RestTimeSeconds = we.RestTimeSeconds
                    })
                    .ToList()
            }).ToList();
        }
    }
}
