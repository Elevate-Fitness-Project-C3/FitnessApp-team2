using MediatR;
using TrainingService.Repositories.Interfaces;
using ProgramWorkoutEntity = TrainingService.Features.ProgramWorkout.Domain.Entities.ProgramWorkout;

namespace TrainingService.Features.ProgramWorkout.Create;

public record CreateProgramWorkoutCommand(
    int ProgramId,
    int WorkoutId,
    int WeekNumber,
    int DayNumber);

public record CreateProgramWorkoutEntityCommand(
    int ProgramId,
    int WorkoutId,
    int WeekNumber,
    int DayNumber) : IRequest<int>;

public sealed class CreateProgramWorkoutEntityCommandHandler
    : IRequestHandler<CreateProgramWorkoutEntityCommand, int>
{
    private readonly IGenericRepository<ProgramWorkoutEntity> _repository;

    public CreateProgramWorkoutEntityCommandHandler(
        IGenericRepository<ProgramWorkoutEntity> repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(
        CreateProgramWorkoutEntityCommand request,
        CancellationToken cancellationToken)
    {
        var programWorkout = new ProgramWorkoutEntity
        {
            ProgramId = request.ProgramId,
            WorkoutId = request.WorkoutId,
            WeekNumber = request.WeekNumber,
            DayNumber = request.DayNumber
        };

        _repository.Add(programWorkout);
        await _repository.SaveChangesAsync(cancellationToken);

        return programWorkout.Id;
    }
}
