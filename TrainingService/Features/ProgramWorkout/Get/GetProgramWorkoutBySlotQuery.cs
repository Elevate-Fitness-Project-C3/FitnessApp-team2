using MediatR;
using TrainingService.Features.ProgramWorkout.Domain.Entities;
using TrainingService.Features.ProgramWorkout.Get;
using TrainingService.Repositories.Interfaces;
using TrainingService.TrainingProgram.Entities;

namespace TrainingService.TrainingProgram.Queries
{
    public record GetProgramWorkoutBySlotQuery(
        int ProgramId,
        int WeekNumber,
        int DayNumber
    ) : IRequest<GetProgramWorkoutBySlotQueryDTO?>;
    public class GetProgramWorkoutBySlotQueryHandler
      : IRequestHandler<GetProgramWorkoutBySlotQuery,
          GetProgramWorkoutBySlotQueryDTO?>
    {
        private readonly IGenericRepository<ProgramWorkout> _repository;

        public GetProgramWorkoutBySlotQueryHandler(
            IGenericRepository<ProgramWorkout> repository)
        {
            _repository = repository;
        }

        public async Task<GetProgramWorkoutBySlotQueryDTO?> Handle(
            GetProgramWorkoutBySlotQuery request,
            CancellationToken cancellationToken)
        {
            var programWorkout = await _repository.FirstOrDefualtAsyc(
                pw => pw.ProgramId == request.ProgramId &&
                      pw.WeekNumber == request.WeekNumber &&
                      pw.DayNumber == request.DayNumber,
                ct: cancellationToken
            );

            return programWorkout is not null
                ? new GetProgramWorkoutBySlotQueryDTO
                {
                    ProgramId = programWorkout.ProgramId,
                    WeekNumber = programWorkout.WeekNumber,
                    DayNumber = programWorkout.DayNumber
                }
                : null;
        }
    }
}