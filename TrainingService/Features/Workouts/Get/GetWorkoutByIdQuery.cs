using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Features.Workouts.Get;
using TrainingService.Repositories.Interfaces;
using TrainingService.Workout.Entities;

namespace TrainingService.TrainingProgram.Queries
{
    public record GetWorkoutByIdQuery(int WorkoutId)
        : IRequest<GetWorkoutByIdQueryDTO?>;

    public class GetWorkoutByIdQueryHandler
        : IRequestHandler<GetWorkoutByIdQuery, GetWorkoutByIdQueryDTO?>
    {
        private readonly IGenericRepository<Workout.Entities.Workout> _repository;

        public GetWorkoutByIdQueryHandler(
            IGenericRepository<Workout.Entities.Workout> repository)
        {
            _repository = repository;
        }

        public async Task<GetWorkoutByIdQueryDTO?> Handle(
            GetWorkoutByIdQuery request,
            CancellationToken cancellationToken)
        {
            var workout = await _repository.GetAll()
                .Include(w => w.WorkoutExercises)
                    .ThenInclude(we => we.Exercise)
                .FirstOrDefaultAsync(w => w.Id == request.WorkoutId, cancellationToken);

            if (workout == null)
                return null;

            return new GetWorkoutByIdQueryDTO
            {
                Id = workout.Id,
                Title = workout.Title,
                Description = workout.Description,
                Difficulty = workout.Difficulty,
                EstimatedDurationMinutes = workout.EstimatedDurationMinutes,
                Exercises = workout.WorkoutExercises
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
            };
        }
    }
}
