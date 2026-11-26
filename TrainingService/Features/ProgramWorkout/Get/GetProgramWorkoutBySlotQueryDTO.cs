namespace TrainingService.Features.ProgramWorkout.Get
{
    public class GetProgramWorkoutBySlotQueryDTO
    {
        public int ProgramId { get; set; }
        public int WeekNumber { get; set; }
        public int DayNumber { get; set; }
    }
}
