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
        private readonly IUnitOfWork _unitOfWork;

        public DeleteWorkoutCommandHandler(
            IGenericRepository<Entities.Workout> workoutRepository,
            IUnitOfWork unitOfWork)
        {
            _workoutRepository = workoutRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteWorkoutCommand request, CancellationToken cancellationToken)
        {
            // 1. Check if workout exists (using single generic repo, NO Include)
            var workout = await _workoutRepository.GetByIdAsync(request.WorkoutId, cancellationToken);
            if (workout == null)
            {
                throw new KeyNotFoundException($"Workout with ID {request.WorkoutId} was not found.");
            }

            // 2. AC 05 Guard: Cannot delete workout used in published programs (NO Include)
            var programWorkoutRepo = _unitOfWork.Repository<ProgramWorkout>();
            var isLinkedToPublishedProgram = await programWorkoutRepo.GetAll()
                .AnyAsync(pw => pw.WorkoutId == request.WorkoutId && pw.Program.Status == ProgramStatus.Published, cancellationToken);

            if (isLinkedToPublishedProgram)
            {
                throw new WorkoutPublishedException(
                    $"Workout with ID {request.WorkoutId} cannot be deleted because it is part of a published training program schedule.");
            }

            // 3. Atomically delete via Unit of Work with ExecuteAsync
            await _unitOfWork.ExecuteAsync(async () =>
            {
                _workoutRepository.Delete(workout);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            });

            return Unit.Value;
        }
    }
}
