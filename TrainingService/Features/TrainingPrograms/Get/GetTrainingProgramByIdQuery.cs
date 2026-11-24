using global::TrainingService.Repositories.Interfaces;
using MediatR;
using TrainingService.Features.TrainingPrograms.Get;
using TrainingService.Features.TrainingPrograms;
using TrainingService.TrainingProgram.Entities;


    namespace TrainingService.Features.TrainingPrograms.Get
    {
        public record GetTrainingProgramByIdQuery(int ProgramId)
            : IRequest<GetTrainingProgramDTO>;

        public class GetTrainingProgramByIdQueryHandler
            : IRequestHandler<GetTrainingProgramByIdQuery, GetTrainingProgramDTO?>
        {
            private readonly IGenericRepository<TrainingService.Features.TrainingPrograms.Domain.Entities.TrainingProgram> _repository;

            public GetTrainingProgramByIdQueryHandler(
                IGenericRepository<TrainingService.Features.TrainingPrograms.Domain.Entities.TrainingProgram> repository)
            {
                _repository = repository;
            }

            public async Task<GetTrainingProgramDTO?> Handle(
                GetTrainingProgramByIdQuery request,
                CancellationToken cancellationToken)
            {
                var trainingProgram = await _repository.GetByIdAsync(request.ProgramId);
                if (trainingProgram == null)
                    return null;

                return new GetTrainingProgramDTO
                {
                    Id = trainingProgram.Id,
                    Goal = trainingProgram.Goal,
                    Level = trainingProgram.Level,
                    DurationWeeks = trainingProgram.DurationWeeks,
                    DaysPerWeek = trainingProgram.DaysPerWeek,
                    AccessTier = trainingProgram.AccessTier,
                    Description = trainingProgram.Description,
                    Status = trainingProgram.Status
                };
            }
        }
    }

