using MediatR;
using TrainingService.Features.ProgramWorkout.Get;
using TrainingService.Features.TrainingPrograms.Get;
using TrainingService.TrainingProgram.Queries;

namespace TrainingService.Features.ProgramWorkout.Create;

public sealed class CreateProgramWorkoutOrchestrator
{
    private readonly IMediator _mediator;

    public CreateProgramWorkoutOrchestrator(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<int> Execute(
        CreateProgramWorkoutCommand request,
        CancellationToken cancellationToken)
    {
        var program = await _mediator.Send(
            new GetTrainingProgramByIdQuery(request.ProgramId),
            cancellationToken);

        if (program is null)
            throw new KeyNotFoundException("Program not found.");

        var workout = await _mediator.Send(
            new GetWorkoutByIdQuery(request.WorkoutId),
            cancellationToken);

        if (workout is null)
            throw new KeyNotFoundException("Workout not found.");

        if (request.WeekNumber < 1 || request.WeekNumber > program.DurationWeeks)
            throw new ArgumentException("WeekNumber must be between 1 and DurationWeeks.");

        if (request.DayNumber < 1 || request.DayNumber > program.DaysPerWeek)
            throw new ArgumentException("DayNumber must be between 1 and DaysPerWeek.");

        var existingSlot = await _mediator.Send(
            new GetProgramWorkoutBySlotQuery(
                request.ProgramId,
                request.WeekNumber,
                request.DayNumber),
            cancellationToken);

        if (existingSlot is not null)
            throw new InvalidOperationException("This slot already has a workout assigned.");

        return await _mediator.Send(
            new CreateProgramWorkoutEntityCommand(
                request.ProgramId,
                request.WorkoutId,
                request.WeekNumber,
                request.DayNumber),
            cancellationToken);
    }
}

