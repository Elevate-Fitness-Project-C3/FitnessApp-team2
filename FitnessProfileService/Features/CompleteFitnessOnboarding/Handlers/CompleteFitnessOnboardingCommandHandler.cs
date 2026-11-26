using FitnessProfileService.Common.Responses;
using FitnessProfileService.Domain.Contracts;
using FitnessProfileService.Domain.Entities;
using FitnessProfileService.Domain.Enums;
using FitnessProfileService.Features.CompleteFitnessOnboarding.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitnessProfileService.Features.CompleteFitnessOnboarding.Handlers
{
    public class CompleteFitnessOnboardingCommandHandler
        : IRequestHandler<CompleteFitnessOnboardingCommand, RequestResponse<bool>>
    {
        private readonly IGenericRepository<FitnessProfile> _fitnessProfileRepository;

        public CompleteFitnessOnboardingCommandHandler(IGenericRepository<FitnessProfile> fitnessProfileRepository)
        {
            _fitnessProfileRepository = fitnessProfileRepository;
        }

        public async Task<RequestResponse<bool>> Handle(CompleteFitnessOnboardingCommand request, CancellationToken cancellationToken)
        {
            // 1. Get existing profile
            var profile = await _fitnessProfileRepository.GetAll().FirstOrDefaultAsync(p => request.UserId == p.UserId);

            // 2. Check existence
            if (profile == null)
            {
                return RequestResponse<bool>.Fail(
                    "Fitness profile not found",404);
            }

            // 3. Check onboarding eligibility
            if (profile.OnboardingStatus == OnboardingStatus.Completed)
            {
                return RequestResponse<bool>.Fail(
                    "Onboarding has already been completed",409);
            }

            // 4. Update profile properties
            profile.Gender = request.Gender;
            profile.DateOfBirth = request.DateOfBirth;
            profile.BodyMeasurements.Add( new BodyMeasurement()
            {
                WeightKg = request.WeightKg,
                HeightCm = request.HeightCm,
                RecordedAt = DateTime.UtcNow
            });
            profile.PrimaryFitnessGoal = request.PrimaryFitnessGoal;
            profile.ActivityLevel = request.PhysicalActivityLevel;
            profile.UpdatedAt = DateTime.UtcNow;
            // 5. Complete onboarding
            profile.OnboardingStatus = OnboardingStatus.Completed;

            // 6. Save
            await _fitnessProfileRepository.SaveChangesAsync(cancellationToken);

            return RequestResponse<bool>.Ok(true);
        }
    }
}
