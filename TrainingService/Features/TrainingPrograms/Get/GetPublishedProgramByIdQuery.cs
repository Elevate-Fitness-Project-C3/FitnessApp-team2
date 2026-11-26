using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Features.TrainingPrograms.DTOs;
using TrainingService.Repositories.Interfaces;
using static TrainingService.TrainingProgram.Enums.Enums;
using TrainingProgramEntity = TrainingService.Features.TrainingPrograms.Domain.Entities.TrainingProgram;

namespace TrainingService.Features.TrainingPrograms.Get
{
    public record GetPublishedProgramByIdQuery(
        int ProgramId,
        bool HasPremiumAccess = false
    ) : IRequest<PublishedProgramDetailsDTO?>;

    public class GetPublishedProgramByIdQueryHandler : IRequestHandler<GetPublishedProgramByIdQuery, PublishedProgramDetailsDTO?>
    {
        private readonly IGenericRepository<TrainingProgramEntity> _repository;

        public GetPublishedProgramByIdQueryHandler(IGenericRepository<TrainingProgramEntity> repository)
        {
            _repository = repository;
        }

        public async Task<PublishedProgramDetailsDTO?> Handle(GetPublishedProgramByIdQuery request, CancellationToken cancellationToken)
        {
            // Direct projection via Select without eager Include overhead
            var program = await _repository.GetAll()
                .AsNoTracking()
                .Where(p => p.Id == request.ProgramId && p.Status == ProgramStatus.Published)
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Goal,
                    p.Level,
                    p.DurationWeeks,
                    p.DaysPerWeek,
                    p.AccessTier,
                    p.Description,
                    Status = p.Status.ToString(),
                    Schedule = p.ProgramWorkouts
                        .OrderBy(pw => pw.WeekNumber)
                        .ThenBy(pw => pw.DayNumber)
                        .Select(pw => new
                        {
                            pw.WeekNumber,
                            pw.DayNumber,
                            Workout = pw.Workout != null ? new
                            {
                                WorkoutId = pw.Workout.Id,
                                Title = pw.Workout.Title,
                                Difficulty = pw.Workout.Difficulty,
                                EstimatedDurationMinutes = pw.Workout.EstimatedDurationMinutes,
                                Exercises = pw.Workout.WorkoutExercises
                                    .OrderBy(we => we.OrderIndex)
                                    .Select(we => new ProgramWorkoutExerciseDTO
                                    {
                                        Id = we.Id,
                                        ExerciseId = we.ExerciseId,
                                        ExerciseName = we.Exercise != null ? we.Exercise.Name : string.Empty,
                                        OrderIndex = we.OrderIndex,
                                        Sets = we.Sets,
                                        Reps = we.Reps,
                                        DurationSeconds = we.DurationSeconds,
                                        RestTimeSeconds = we.RestTimeSeconds
                                    }).ToList()
                            } : null
                        }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (program == null)
                return null;

            return new PublishedProgramDetailsDTO
            {
                Id = program.Id,
                Title = program.Title,
                Goal = program.Goal,
                Level = program.Level,
                DurationWeeks = program.DurationWeeks,
                DaysPerWeek = program.DaysPerWeek,
                AccessTier = program.AccessTier.ToString(),
                IsLocked = program.AccessTier == AccessTier.Premium && !request.HasPremiumAccess,
                Description = program.Description,
                Status = program.Status,
                Schedule = program.Schedule.Select(s => new ProgramSlotDTO
                {
                    WeekNumber = s.WeekNumber,
                    DayNumber = s.DayNumber,
                    Workout = s.Workout != null ? new ProgramWorkoutSummaryDTO
                    {
                        WorkoutId = s.Workout.WorkoutId,
                        Title = s.Workout.Title,
                        Difficulty = s.Workout.Difficulty,
                        EstimatedDurationMinutes = s.Workout.EstimatedDurationMinutes,
                        Exercises = s.Workout.Exercises
                    } : null
                }).ToList()
            };
        }
    }
}
