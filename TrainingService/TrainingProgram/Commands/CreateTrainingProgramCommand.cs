using MediatR;
using TrainingService.Repositories.Interfaces;
using TrainingService.TrainingProgram.DTOs;
using TrainingService.TrainingProgram.Validation;
using static TrainingService.TrainingProgram.Enums.Enums;

namespace TrainingService.TrainingProgram.Commands
{
    public record CreateTrainingProgramCommand
    ( string Goal, string Level,  int DurationWeeks, int DaysPerWeek, AccessTier AccessTier,string Description) : IRequest<CreateTrainingProgramDTO>;


    public class CreateTrainingProgramCommandHnadler : IRequestHandler<CreateTrainingProgramCommand, CreateTrainingProgramDTO>
    {
        private readonly IGenericRepository<Entities.TrainingProgram> _repository;

        public CreateTrainingProgramCommandHnadler(IGenericRepository<Entities.TrainingProgram> repository)
        {
            _repository = repository;
        }

        public async Task<CreateTrainingProgramDTO> Handle(CreateTrainingProgramCommand request, CancellationToken cancellationToken)
        {
            TrainingProgramValidator.Validate(request);

            var trainingProgram = new Entities.TrainingProgram
            {
                Goal = request.Goal,
                Level = request.Level,
                DurationWeeks = request.DurationWeeks,
                DaysPerWeek = request.DaysPerWeek,
                AccessTier = request.AccessTier,
                Description = request.Description,
                Status = ProgramStatus.Draft
            };

            _repository.Add(trainingProgram);
            await _repository.SaveChangesAsync(cancellationToken);

            return await Task.FromResult(new CreateTrainingProgramDTO
            {
                ProgramId = trainingProgram.Id,
                Goal = trainingProgram.Goal,
                Level = trainingProgram.Level,
                DurationWeeks = trainingProgram.DurationWeeks,
                DaysPerWeek = trainingProgram.DaysPerWeek,
                AccessTier = trainingProgram.AccessTier,
                Description = trainingProgram.Description,
                Status = trainingProgram.Status
            });
        }
    }
}
