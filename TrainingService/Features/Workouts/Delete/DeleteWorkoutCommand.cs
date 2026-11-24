using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Features.ProgramWorkout.Domain.Entities;
using TrainingService.Repositories.Interfaces;
using TrainingService.Workout.Commands;
using TrainingService.Workout.Entities;
using static TrainingService.TrainingProgram.Enums.Enums;

namespace TrainingService.Workout.Commands
{
    public record DeleteWorkoutCommand(int WorkoutId) : IRequest;

    public class DeleteWorkoutCommandHandler : IRequestHandler<DeleteWorkoutCommand>
    {
        private readonly IGenericRepository<Entities.Workout> _workoutRepository;
        private readonly IGenericRepository<ProgramWorkout> _programWorkoutRepository;

        public DeleteWorkoutCommandHandler(
            IGenericRepository<Entities.Workout> workoutRepository,
            IGenericRepository<ProgramWorkout> programWorkoutRepository)
        {
            _workoutRepository = workoutRepository;
            _programWorkoutRepository = programWorkoutRepository;
        }

        public async Task<Unit> Handle(DeleteWorkoutCommand request, CancellationToken cancellationToken)
        {
            var workout = await _workoutRepository.GetByIdAsync(request.WorkoutId, cancellationToken);
            if (workout == null)
            {
                throw new KeyNotFoundException($"Workout with ID {request.WorkoutId} was not found.");
            }

            // AC 05 Guard: Cannot delete workout used in published programs
            var isLinkedToPublishedProgram = await _programWorkoutRepository.GetAll()
                .Include(pw => pw.Program)
                .AnyAsync(pw => pw.WorkoutId == request.WorkoutId && pw.Program.Status == ProgramStatus.Published, cancellationToken);

            if (isLinkedToPublishedProgram)
            {
                throw new WorkoutPublishedException(
                    $"Workout with ID {request.WorkoutId} cannot be deleted because it is part of a published training program schedule.");
            }

            _workoutRepository.Delete(workout);
            await _workoutRepository.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
