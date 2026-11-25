using MediatR;
using TrainingService.TrainingProgram.Commands;
using TrainingService.Validation;

namespace TrainingService.TrainingProgram.EndPoints
{
    public static class CreateTrainingProgramEndPoint
    {
        public static void MapCreateTrainingProgramEndPoint(this IEndpointRouteBuilder app  )
        {
            app.MapPost("/api/trainingPrograms", async (CreateTrainingProgramCommand command, IMediator mediator) =>
            {
                try
                {
                    var trainingProgramId = await mediator.Send(command);
                    return Results.Created($"/api/trainingPrograms/{trainingProgramId}", trainingProgramId);
                }
                catch (RequestValidationException exception)
                {
                    return Results.ValidationProblem(exception.Errors);
                }
            });
        }
    }
}
