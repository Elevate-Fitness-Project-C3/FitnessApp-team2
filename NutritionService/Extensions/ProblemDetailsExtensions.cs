namespace NutritionService.Extensions;

public static class ProblemDetailsExtensions
{
    public static IServiceCollection AddNutritionProblemDetails(
        this IServiceCollection services)
    {
        services.AddProblemDetails();

        services.AddExceptionHandler<ValidationExceptionHandler>();

        return services;
    }

    public static WebApplication UseNutritionProblemDetails(
        this WebApplication app)
    {
        app.UseExceptionHandler();

        app.UseStatusCodePages();

        return app;
    }
}