using MediatR;
using TrainingService.Repositories.Interfaces;
using TrainingService.TrainingProgram.DTOs;
using TrainingService.TrainingProgram.Validation;
using static TrainingService.TrainingProgram.Enums.Enums;

namespace TrainingService.TrainingProgram.Commands
{
    public record CreateTrainingProgramCommand
    ( string Goal, string Level,  int DurationWeeks, int DaysPerWeek, AccessTier AccessTier,string Description) : IRequest<int>;


    public class CreateTrainingProgramCommandHnadler : IRequestHandler<CreateTrainingProgramCommand, int>
    {
        private readonly IGenericRepository<Features.TrainingPrograms.Domain.Entities.TrainingProgram> _repository;

        public CreateTrainingProgramCommandHnadler(IGenericRepository<Features.TrainingPrograms.Domain.Entities.TrainingProgram> repository)
        {
            _repository = repository;
        }

        public async Task<int> Handle(CreateTrainingProgramCommand request, CancellationToken cancellationToken)
        {
            TrainingProgramValidator.Validate(request);

            var trainingProgram = new Features.TrainingPrograms.Domain.Entities.TrainingProgram 
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

            return  trainingProgram.Id;
        }
    }
}
