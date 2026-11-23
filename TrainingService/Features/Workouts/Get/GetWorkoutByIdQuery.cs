using MediatR;
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
            var workout = await _repository.GetByIdAsync(request.WorkoutId);
            if (workout == null)
                return null;

            return new GetWorkoutByIdQueryDTO
            {
                Id = workout.Id,
                Title = workout.Title,
                Description = workout.Description,
                Difficulty = workout.Difficulty,
                EstimatedDurationMinutes = workout.EstimatedDurationMinutes
            };
        }
    }
}