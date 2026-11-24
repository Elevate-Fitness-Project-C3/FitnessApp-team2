using FitnessProfileService.Common.Responses;
using FitnessProfileService.Domain.Contracts;
using FitnessProfileService.Domain.Entities;
using FitnessProfileService.Features.CompleteFitnessOnboarding.DTOs;
using FitnessProfileService.Features.CompleteFitnessOnboarding.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitnessProfileService.Features.CompleteFitnessOnboarding.Handlers
{
    public class CheckOnboardingEligibilityQueryHandler :
                    IRequestHandler<CheckOnboardingEligibilityQuery, RequestResponse<OnboardingEligibilityDto>>
    {
        private readonly IGenericRepository<FitnessProfile> _fitnessProfileRepo;

        public CheckOnboardingEligibilityQueryHandler(IGenericRepository<FitnessProfile> fitnessProfileRepo)
        {
            _fitnessProfileRepo = fitnessProfileRepo;
        }

        public async Task<RequestResponse<OnboardingEligibilityDto>> Handle(CheckOnboardingEligibilityQuery request, CancellationToken cancellationToken)
        {
            var onboardingEligibilityDto = await _fitnessProfileRepo.GetAll()
                                        .Where(p => p.UserId == request.UserId)
                                        .Select(x => new OnboardingEligibilityDto(
                                                                true,
                                                                x.OnboardingStatus))
                                                            .FirstOrDefaultAsync(cancellationToken)
                                                            ?? new OnboardingEligibilityDto(false, null);
            return RequestResponse<OnboardingEligibilityDto>.Ok(onboardingEligibilityDto);

        }
    }
}
