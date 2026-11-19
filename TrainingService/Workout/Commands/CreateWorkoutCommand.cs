using MediatR;
using TrainingService.Repositories.Interfaces;
using TrainingService.Workout.DTOs;
using TrainingService.Workout.Entitis;
using TrainingService.Workout.Validation;

namespace TrainingService.Workout.Commands
{
    public record CreateWorkoutCommand( string Title, string Description,  string Difficulty, string EstimatedDurationMinutes ) :IRequest<CreateWorkoutDTO>;


   public class CreateWorkoutCommandHandler : IRequestHandler<CreateWorkoutCommand, CreateWorkoutDTO>
    {
        private readonly IGenericRepository<Entitis.Workout> _repository;
        public CreateWorkoutCommandHandler(IGenericRepository<Entitis.Workout> workoutRepository)
        {
            _repository = workoutRepository;
        }
        public async Task<CreateWorkoutDTO> Handle(CreateWorkoutCommand request, CancellationToken cancellationToken)
        {
            WorkoutValidator.Validate(request);

            var workout = new Entitis.Workout
            {
                Title = request.Title,
                Description = request.Description,
                Difficulty = request.Difficulty,
                EstimatedDurationMinutes = request.EstimatedDurationMinutes
            };
            _repository.Add(workout);
            await _repository.SaveChangesAsync(cancellationToken);

            return await Task.FromResult(new CreateWorkoutDTO
            {
                Id = workout.Id,
                Title = workout.Title,
                Description = workout.Description,
                Difficulty = workout.Difficulty,
                EstimatedDurationMinutes = workout.EstimatedDurationMinutes
            });
        }
    }
}
