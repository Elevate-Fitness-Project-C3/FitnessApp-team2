namespace TrainingService.Features.Exercises.DeactivateExercise;

using MediatR;

public record DeactivateExerciseCommand(Guid Id) : IRequest<DeactivateExerciseResult>;
