namespace NutritionService.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapNutritionEndpoints(
        this IEndpointRouteBuilder app)
    {
        NutritionService.Features.NutritionGoals.SetNutritionGoal.Endpoint
            .MapEndpoint(app);

        return app;
    }
}