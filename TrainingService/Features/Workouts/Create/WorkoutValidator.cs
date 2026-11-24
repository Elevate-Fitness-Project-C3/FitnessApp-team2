using TrainingService.Validation;
using TrainingService.Workout.DTOs;

namespace TrainingService.Workout.Validation
{
    public static class WorkoutValidator
    {
        public static void Validate(
            string title,
            string description,
            string difficulty,
            int estimatedDurationMinutes,
            IList<WorkoutExerciseItem>? exercises)
        {
            var errors = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(title))
            {
                errors[nameof(title)] = ["Title is required."];
            }
            else if (title.Trim().Length > 200)
            {
                errors[nameof(title)] = ["Title cannot exceed 200 characters."];
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                errors[nameof(description)] = ["Description is required."];
            }
            else if (description.Trim().Length > 2000)
            {
                errors[nameof(description)] = ["Description cannot exceed 2000 characters."];
            }

            if (string.IsNullOrWhiteSpace(difficulty))
            {
                errors[nameof(difficulty)] = ["Difficulty is required."];
            }

            if (estimatedDurationMinutes <= 0)
            {
                errors[nameof(estimatedDurationMinutes)] = ["EstimatedDurationMinutes must be a positive whole number greater than zero."];
            }

            // AC 03: Reject empty workout definitions
            if (exercises == null || exercises.Count == 0)
            {
                errors["Exercises"] = ["A workout must contain at least one exercise entry."];
            }
            else
            {
                // AC 03: Reject duplicate order values within a workout
                var duplicateOrders = exercises
                    .GroupBy(e => e.OrderIndex)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicateOrders.Count > 0)
                {
                    errors["Exercises.OrderIndex"] = [$"Duplicate OrderIndex values are not allowed within a workout: {string.Join(", ", duplicateOrders)}."];
                }

                // AC 02: Validate each entry's targets
                for (int i = 0; i < exercises.Count; i++)
                {
                    var ex = exercises[i];
                    var prefix = $"Exercises[{i}]";

                    if (ex.ExerciseId == Guid.Empty)
                    {
                        errors[$"{prefix}.ExerciseId"] = ["ExerciseId must be a valid non-empty GUID."];
                    }

                    if (ex.OrderIndex <= 0)
                    {
                        errors[$"{prefix}.OrderIndex"] = ["OrderIndex must be a positive number greater than zero."];
                    }

                    if (ex.Sets <= 0)
                    {
                        errors[$"{prefix}.Sets"] = ["Sets must be greater than zero."];
                    }

                    // Must include relevant targets (sets, reps, or duration)
                    bool hasReps = ex.Reps.HasValue && ex.Reps.Value > 0;
                    bool hasDuration = ex.DurationSeconds.HasValue && ex.DurationSeconds.Value > 0;

                    if (!hasReps && !hasDuration)
                    {
                        errors[$"{prefix}.Targets"] = ["Each exercise entry must specify target Reps or DurationSeconds greater than zero."];
                    }

                    if (ex.Reps.HasValue && ex.Reps.Value <= 0)
                    {
                        errors[$"{prefix}.Reps"] = ["Reps must be greater than zero when specified."];
                    }

                    if (ex.DurationSeconds.HasValue && ex.DurationSeconds.Value <= 0)
                    {
                        errors[$"{prefix}.DurationSeconds"] = ["DurationSeconds must be greater than zero when specified."];
                    }

                    if (ex.RestTimeSeconds.HasValue && ex.RestTimeSeconds.Value < 0)
                    {
                        errors[$"{prefix}.RestTimeSeconds"] = ["RestTimeSeconds cannot be negative."];
                    }
                }
            }

            if (errors.Count > 0)
            {
                throw new RequestValidationException(errors);
            }
        }
    }
}
