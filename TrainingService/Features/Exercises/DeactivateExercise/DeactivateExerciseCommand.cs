namespace TrainingService.Features.Exercises.DeactivateExercise;

using MediatR;
using TrainingService.Common.Results;

public record DeactivateExerciseCommand(Guid Id) : IRequest<Result>;
