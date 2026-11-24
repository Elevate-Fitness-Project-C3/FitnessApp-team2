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
        private readonly IUnitOfWork _unitOfWork;

        public UpdateWorkoutCommandHandler(
            IGenericRepository<Entities.Workout> workoutRepository,
            IUnitOfWork unitOfWork)
        {
            _workoutRepository = workoutRepository;
            _unitOfWork = unitOfWork;
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

            // 2. Check if workout exists (using single generic repo, NO Include)
            var workout = await _workoutRepository.GetByIdAsync(request.WorkoutId, cancellationToken);
            if (workout == null)
            {
                throw new KeyNotFoundException($"Workout with ID {request.WorkoutId} was not found.");
            }

            // 3. AC 05 Guard: Changes to reusable workouts must not silently alter a published program schedule
            // Direct query using ProgramWorkout navigation in predicate without eager Include
            var programWorkoutRepo = _unitOfWork.Repository<ProgramWorkout>();
            var isLinkedToPublishedProgram = await programWorkoutRepo.GetAll()
                .AnyAsync(pw => pw.WorkoutId == request.WorkoutId && pw.Program.Status == ProgramStatus.Published, cancellationToken);

            if (isLinkedToPublishedProgram)
            {
                throw new WorkoutPublishedException(
                    $"Workout with ID {request.WorkoutId} cannot be modified because it is currently linked to a published training program schedule.");
            }

            // 5. Validate that all exercise entries refer to existing ACTIVE exercises (AC 02)
            var exerciseRepo = _unitOfWork.Repository<Exercise>();
            var distinctExerciseIds = request.Exercises.Select(e => e.ExerciseId).Distinct().ToList();
            var activeExerciseIds = await exerciseRepo.GetAll()
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

            // 6. Update workout and exercises atomically using Unit of Work ExecuteAsync
            await _unitOfWork.ExecuteAsync(async () =>
            {
                // Update scalar properties
                workout.Title = request.Title.Trim();
                workout.Description = request.Description.Trim();
                workout.Difficulty = request.Difficulty.Trim();
                workout.EstimatedDurationMinutes = request.EstimatedDurationMinutes;

                _workoutRepository.Update(workout);

                // Replace existing exercises
                var workoutExerciseRepo = _unitOfWork.Repository<WorkoutExercise>();
                var existingExercises = await workoutExerciseRepo.GetAll(tracking: true)
                    .Where(we => we.WorkoutId == workout.Id)
                    .ToListAsync(cancellationToken);

                foreach (var ex in existingExercises)
                {
                    workoutExerciseRepo.Delete(ex);
                }

                foreach (var item in request.Exercises.OrderBy(e => e.OrderIndex))
                {
                    workoutExerciseRepo.Add(new WorkoutExercise
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

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            });

            return Unit.Value;
        }
    }
}
