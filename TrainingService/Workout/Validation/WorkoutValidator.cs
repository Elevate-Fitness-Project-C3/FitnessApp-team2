using TrainingService.Validation;
using TrainingService.Workout.Commands;

namespace TrainingService.Workout.Validation
{
    public static class WorkoutValidator
    {
        public static void Validate(CreateWorkoutCommand request)
        {
            var errors = new Dictionary<string, string[]>();

            AddRequiredError(errors, nameof(request.Title), request.Title, "Title is required.");
            AddRequiredError(errors, nameof(request.Description), request.Description, "Description is required.");
            AddRequiredError(errors, nameof(request.Difficulty), request.Difficulty, "Difficulty is required.");

            if (!int.TryParse(request.EstimatedDurationMinutes, out var durationMinutes) || durationMinutes <= 0)
            {
                errors[nameof(request.EstimatedDurationMinutes)] =
                ["EstimatedDurationMinutes must be a positive whole number."];
            }

            if (errors.Count > 0)
            {
                throw new RequestValidationException(errors);
            }
        }

        private static void AddRequiredError(
            IDictionary<string, string[]> errors,
            string propertyName,
            string? value,
            string message)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                errors[propertyName] = [message];
            }
        }
    }
}
