using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Features.ProgramWorkout.Domain.Entities;
using TrainingService.Repositories.Interfaces;
using TrainingService.Validation;
using TrainingService.Workout.DTOs;
using TrainingService.Workout.Entities;
using TrainingService.Workout.Validation;
using static TrainingService.TrainingProgram.Enums.Enums;

namespace TrainingService.Workout.Commands
{
    public class WorkoutPublishedException : Exception
    {
        public WorkoutPublishedException(string message) : base(message) { }
    }

    public record UpdateWorkoutCommand(
        int WorkoutId,
        string Title,
        string Description,
        string Difficulty,
        int EstimatedDurationMinutes,
        List<WorkoutExerciseItem> Exercises
    ) : IRequest;

    public class UpdateWorkoutCommandHandler : IRequestHandler<UpdateWorkoutCommand>
    {
        private readonly IGenericRepository<Entities.Workout> _workoutRepository;
        private readonly IGenericRepository<Exercise> _exerciseRepository;
        private readonly IGenericRepository<ProgramWorkout> _programWorkoutRepository;

        public UpdateWorkoutCommandHandler(
            IGenericRepository<Entities.Workout> workoutRepository,
            IGenericRepository<Exercise> exerciseRepository,
            IGenericRepository<ProgramWorkout> programWorkoutRepository)
        {
            _workoutRepository = workoutRepository;
            _exerciseRepository = exerciseRepository;
            _programWorkoutRepository = programWorkoutRepository;
        }

        public async Task<Unit> Handle(UpdateWorkoutCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate inputs (AC 01, AC 03)
            WorkoutValidator.Validate(
                request.Title,
                request.Description,
                request.Difficulty,
                request.EstimatedDurationMinutes,
                request.Exercises);

            // 2. Fetch existing workout with tracked exercises
            var workout = await _workoutRepository.GetAll(tracking: true)
                .Include(w => w.WorkoutExercises)
                .FirstOrDefaultAsync(w => w.Id == request.WorkoutId, cancellationToken);

            if (workout == null)
            {
                throw new KeyNotFoundException($"Workout with ID {request.WorkoutId} was not found.");
            }

            // 3. AC 05 Guard: Changes to reusable workouts must not silently alter a published program schedule
            // Design checkpoint (mentor hint): Do not change reusable structures behind already published program versions.
            var isLinkedToPublishedProgram = await _programWorkoutRepository.GetAll()
                .Include(pw => pw.Program)
                .AnyAsync(pw => pw.WorkoutId == request.WorkoutId && pw.Program.Status == ProgramStatus.Published, cancellationToken);

            if (isLinkedToPublishedProgram)
            {
                throw new WorkoutPublishedException(
                    $"Workout with ID {request.WorkoutId} cannot be modified because it is currently linked to a published training program schedule.");
            }

            // 4. Validate that all exercise entries refer to existing ACTIVE exercises (AC 02)
            var distinctExerciseIds = request.Exercises.Select(e => e.ExerciseId).Distinct().ToList();
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

            // 5. Update workout scalar properties
            workout.Title = request.Title.Trim();
            workout.Description = request.Description.Trim();
            workout.Difficulty = request.Difficulty.Trim();
            workout.EstimatedDurationMinutes = request.EstimatedDurationMinutes;

            // 6. Replace workout exercises with updated ordered entries
            workout.WorkoutExercises.Clear();
            foreach (var item in request.Exercises.OrderBy(e => e.OrderIndex))
            {
                workout.WorkoutExercises.Add(new WorkoutExercise
                {
                    WorkoutId = workout.Id,
                    ExerciseId = item.ExerciseId,
                    OrderIndex = item.OrderIndex,
                    Sets = item.Sets,
                    Reps = item.Reps,
                    DurationSeconds = item.DurationSeconds,
                    RestTimeSeconds = item.RestTimeSeconds
                });
            }

            // 7. Persist changes
            await _workoutRepository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
