using MediatR;
using TrainingService.Repositories.Interfaces;
using TrainingService.TrainingProgram.Validation;
using static TrainingService.TrainingProgram.Enums.Enums;

namespace TrainingService.TrainingProgram.Commands
{
    public record CreateTrainingProgramCommand
    (string Title, string Goal, string Level, int DurationWeeks, int DaysPerWeek, AccessTier AccessTier, string Description) : IRequest<int>;

    public class CreateTrainingProgramCommandHandler : IRequestHandler<CreateTrainingProgramCommand, int>
    {
        private readonly IGenericRepository<Features.TrainingPrograms.Domain.Entities.TrainingProgram> _repository;

        public CreateTrainingProgramCommandHandler(IGenericRepository<Features.TrainingPrograms.Domain.Entities.TrainingProgram> repository)
        {
            _repository = repository;
        }

        public async Task<int> Handle(CreateTrainingProgramCommand request, CancellationToken cancellationToken)
        {
            TrainingProgramValidator.Validate(request);

            var trainingProgram = new Features.TrainingPrograms.Domain.Entities.TrainingProgram 
            {
                Title = request.Title,
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

            return trainingProgram.Id;
        }
    }
}
