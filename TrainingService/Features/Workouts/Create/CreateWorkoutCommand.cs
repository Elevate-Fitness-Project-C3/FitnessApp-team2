using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Repositories.Interfaces;
using TrainingService.Validation;
using TrainingService.Workout.DTOs;
using TrainingService.Workout.Entities;
using TrainingService.Workout.Validation;

namespace TrainingService.Workout.Commands
{
    public record CreateWorkoutCommand(
        string Title,
        string Description,
        string Difficulty,
        int EstimatedDurationMinutes,
        List<WorkoutExerciseItem> Exercises
    ) : IRequest<int>;

    public class CreateWorkoutCommandHandler : IRequestHandler<CreateWorkoutCommand, int>
    {
        private readonly IGenericRepository<Entities.Workout> _workoutRepository;
        private readonly IGenericRepository<Exercise> _exerciseRepository;

        public CreateWorkoutCommandHandler(
            IGenericRepository<Entities.Workout> workoutRepository,
            IGenericRepository<Exercise> exerciseRepository)
        {
            _workoutRepository = workoutRepository;
            _exerciseRepository = exerciseRepository;
        }

        public async Task<int> Handle(CreateWorkoutCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate basic rules (AC 01, AC 03)
            WorkoutValidator.Validate(
                request.Title,
                request.Description,
                request.Difficulty,
                request.EstimatedDurationMinutes,
                request.Exercises);

            // 2. Validate that every exercise entry refers to an existing ACTIVE exercise (AC 02)
            var distinctExerciseIds = request.Exercises
                .Select(e => e.ExerciseId)
                .Distinct()
                .ToList();

            var activeExerciseIds = await _exerciseRepository.GetAll()
                .Where(e => distinctExerciseIds.Contains(e.Id) && e.IsActive)
                .Select(e => e.Id)
                .ToListAsync(cancellationToken);

            if (activeExerciseIds.Count != distinctExerciseIds.Count)
            {
                var missingOrInactive = distinctExerciseIds.Except(activeExerciseIds).ToList();
                throw new RequestValidationException(new Dictionary<string, string[]>
                {
                    { "Exercises", [$"The following exercise IDs were not found or are inactive in the catalog: {string.Join(", ", missingOrInactive)}."] }
                });
            }

            // 3. Create Workout domain entity and populate ordered exercises
            var workout = new Entities.Workout
            {
                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                Difficulty = request.Difficulty.Trim(),
                EstimatedDurationMinutes = request.EstimatedDurationMinutes
            };

            foreach (var item in request.Exercises.OrderBy(e => e.OrderIndex))
            {
                workout.WorkoutExercises.Add(new WorkoutExercise
                {
                    ExerciseId = item.ExerciseId,
                    OrderIndex = item.OrderIndex,
                    Sets = item.Sets,
                    Reps = item.Reps,
                    DurationSeconds = item.DurationSeconds,
                    RestTimeSeconds = item.RestTimeSeconds
                });
            }

            // 4. Atomically persist via repository
            _workoutRepository.Add(workout);
            await _workoutRepository.SaveChangesAsync(cancellationToken);

            return workout.Id;
        }
    }
}
