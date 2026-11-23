using static TrainingService.TrainingProgram.Enums.Enums;

namespace TrainingService.TrainingProgram.ViewModels
{
    public class CreateTrainingProgramViewModel
    {
        public string Goal { get; set; }

        public string Level { get; set; } 

        public int DurationWeeks { get; set; }

        public int DaysPerWeek { get; set; }

        public AccessTier AccessTier { get; set; }

        public string Description { get; set; } 

        public ProgramStatus Status { get; set; }
    }
}
