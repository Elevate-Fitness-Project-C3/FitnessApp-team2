using FitnessProfileService.Common.Responses;
using FitnessProfileService.Domain.Contracts;
using FitnessProfileService.Domain.Entities;
using FitnessProfileService.Features.CreateFitnessProfile.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitnessProfileService.Features.CreateFitnessProfile.Handlers
{
    public class CheckFitnessProfileExistsQueryHandler
        : IRequestHandler<CheckFitnessProfileExistsQuery, RequestResponse<bool>>
    {
        private readonly IGenericRepository<FitnessProfile> _fitnessProfileRepo;

        public CheckFitnessProfileExistsQueryHandler(IGenericRepository<FitnessProfile> fitnessProfileRepo)
        {
            _fitnessProfileRepo = fitnessProfileRepo;
        }

        public async Task<RequestResponse<bool>> Handle(CheckFitnessProfileExistsQuery request, CancellationToken cancellationToken)
        {
         var isExist= await  _fitnessProfileRepo.GetAll().AnyAsync(fp => fp.UserId == request.UserId,cancellationToken);
            return  RequestResponse<bool>.Ok(isExist);
        }
    }
}
