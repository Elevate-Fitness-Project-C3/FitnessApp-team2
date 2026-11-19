using MediatR;
using TrainingService.DBContext;
using TrainingService.TrainingProgram.Commands;
using TrainingService.TrainingProgram.ViewModels;
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
                    var trainingProgram = await mediator.Send(command);
                    return Results.Created($"/api/trainingPrograms/{trainingProgram.ProgramId}", trainingProgram);
                }
                catch (RequestValidationException exception)
                {
                    return Results.ValidationProblem(exception.Errors);
                }
            });
        }
    }
}
