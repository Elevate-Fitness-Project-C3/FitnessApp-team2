using MediatR;
using TrainingService.Repositories.Interfaces;
using TrainingService.Workout.DTOs;
using TrainingService.Workout.Entities;
using TrainingService.Workout.Validation;

namespace TrainingService.Workout.Commands
{
    public record CreateWorkoutCommand( string Title, string Description,  string Difficulty, int EstimatedDurationMinutes ) :IRequest<int>;


   public class CreateWorkoutCommandHandler : IRequestHandler<CreateWorkoutCommand, int>
    {
        private readonly IGenericRepository<Entities.Workout> _repository;
        public CreateWorkoutCommandHandler(IGenericRepository<Entities.Workout> workoutRepository)
        {
            _repository = workoutRepository;
        }
        public async Task<int> Handle(CreateWorkoutCommand request, CancellationToken cancellationToken)
        {
            WorkoutValidator.Validate(request);

            var workout = new Entities.Workout
            {
                Title = request.Title,
                Description = request.Description,
                Difficulty = request.Difficulty,
                EstimatedDurationMinutes = request.EstimatedDurationMinutes
            };
            _repository.Add(workout);
            await _repository.SaveChangesAsync(cancellationToken);

            return workout.Id;
        }
    }
}
