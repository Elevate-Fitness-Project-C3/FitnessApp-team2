using TrainingService.TrainingProgram.Commands;
using TrainingService.Validation;

namespace TrainingService.TrainingProgram.Validation
{
    public static class TrainingProgramValidator
    {
        public static void Validate(CreateTrainingProgramCommand request)
        {
            var errors = new Dictionary<string, string[]>();

            AddRequiredError(errors, nameof(request.Title), request.Title, "Title is required.");
            AddRequiredError(errors, nameof(request.Goal), request.Goal, "Goal is required.");
            AddRequiredError(errors, nameof(request.Level), request.Level, "Level is required.");
            AddRequiredError(errors, nameof(request.Description), request.Description, "Description is required.");

            if (!string.IsNullOrWhiteSpace(request.Title) && request.Title.Length > 200)
            {
                errors[nameof(request.Title)] = ["Title cannot exceed 200 characters."];
            }

            if (request.DurationWeeks <= 0)
            {
                errors[nameof(request.DurationWeeks)] = ["DurationWeeks must be greater than zero."];
            }

            if (request.DaysPerWeek is < 1 or > 7)
            {
                errors[nameof(request.DaysPerWeek)] = ["DaysPerWeek must be between 1 and 7."];
            }

            if (!Enum.IsDefined(request.AccessTier))
            {
                errors[nameof(request.AccessTier)] = ["AccessTier is invalid."];
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
