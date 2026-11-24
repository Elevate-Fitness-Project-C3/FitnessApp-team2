namespace TrainingService.Features.Exercises.GetExerciseById;

using MediatR;

public record GetExerciseByIdQuery(Guid Id) : IRequest<ExerciseResponse?>;
