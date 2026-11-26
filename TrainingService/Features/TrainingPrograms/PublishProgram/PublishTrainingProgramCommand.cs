using MediatR;
using TrainingService.Repositories.Interfaces;
using static TrainingService.TrainingProgram.Enums.Enums;
using TrainingProgramEntity = TrainingService.Features.TrainingPrograms.Domain.Entities.TrainingProgram;

namespace TrainingService.Features.TrainingPrograms.PublishProgram
{
    public record PublishTrainingProgramCommand(int ProgramId) : IRequest;

    public class PublishTrainingProgramCommandHandler : IRequestHandler<PublishTrainingProgramCommand>
    {
        private readonly IGenericRepository<TrainingProgramEntity> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public PublishTrainingProgramCommandHandler(
            IGenericRepository<TrainingProgramEntity> repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(PublishTrainingProgramCommand request, CancellationToken cancellationToken)
        {
            var program = await _repository.GetByIdAsync(request.ProgramId, cancellationToken);
            if (program == null)
            {
                throw new KeyNotFoundException($"Training program with ID {request.ProgramId} was not found.");
            }

            await _unitOfWork.ExecuteAsync(async () =>
            {
                program.Status = ProgramStatus.Published;
                _repository.Update(program);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            });

            return Unit.Value;
        }
    }
}
