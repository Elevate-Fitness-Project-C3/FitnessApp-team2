using FitnessProfileService.Common.Responses;
using FitnessProfileService.Domain.Contracts;
using FitnessProfileService.Domain.Entities;
using FitnessProfileService.Features.CreateFitnessProfile.Queries;
using FitnessProfileService.FitnessProfileService.Features.CreateFitnessProfile.Commands;
using MediatR;

namespace FitnessProfileService.Features.CreateFitnessProfile.Handlers
{
    public class CreateFitnessProfileCommandHandler
                 : IRequestHandler<CreateFitnessProfileCommand, RequestResponse<bool>>
    {
        private readonly IGenericRepository<FitnessProfile> _fitnessProfileRepo;
        private IMediator _mediator;

        public CreateFitnessProfileCommandHandler(IGenericRepository<FitnessProfile> fitnessProfileRepo,
            IMediator mediator)
        {
            _fitnessProfileRepo = fitnessProfileRepo;
            _mediator = mediator;
        }

        public async Task<RequestResponse<bool>> Handle(
            CreateFitnessProfileCommand request,
            CancellationToken cancellationToken)
        {
            // Validate UserId
            if (request.UserId == Guid.Empty)
            {
                return RequestResponse<bool>.Fail("Invalid UserId", 400);
            }
            // Check existing profile
            var checkProfileExistsQuery = await _mediator.Send(new CheckFitnessProfileExistsQuery(request.UserId), cancellationToken);

            if (checkProfileExistsQuery is null || !checkProfileExistsQuery.Success)
            {
                return RequestResponse<bool>.Fail("Failed to check existing profile", 500);
            }
            if (checkProfileExistsQuery.Data)
            {
                return RequestResponse<bool>.Ok(true, statusCode: 201);
            }
            ;
            // Create profile
            var fitnessProfile = new FitnessProfile
            {
                UserId = request.UserId,
                CreatedAt = DateTime.UtcNow
            };

            _fitnessProfileRepo.Add(fitnessProfile);
            // Save changes
            await _fitnessProfileRepo.SaveChangesAsync(cancellationToken);
            return RequestResponse<bool>.Ok(true, statusCode: 201);
        }
    }
}
