using static TrainingService.TrainingProgram.Enums.Enums;

namespace TrainingService.Features.TrainingPrograms.Domain.Entities
{
    public class TrainingProgram
    {
            public int Id { get; set; }

            public string Goal { get; set; } = string.Empty;

            public string Level { get; set; } = string.Empty;

            public int DurationWeeks { get; set; }

            public int DaysPerWeek { get; set; }

            public AccessTier AccessTier { get; set; }

            public string Description { get; set; } = string.Empty;

            public ProgramStatus Status { get; set; } = ProgramStatus.Draft;
    }
}
