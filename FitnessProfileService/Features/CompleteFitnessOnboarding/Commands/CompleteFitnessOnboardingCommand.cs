using FitnessProfileService.Common.Responses;
using FitnessProfileService.Domain.Enums;
using MediatR;

namespace FitnessProfileService.Features.CompleteFitnessOnboarding.Commands
{
    public record CompleteFitnessOnboardingCommand(
    Guid UserId,
    Gender Gender,
    DateOnly DateOfBirth,
    decimal HeightCm,
    decimal WeightKg,
    FitnessGoal PrimaryFitnessGoal,
    PhysicalActivityLevel PhysicalActivityLevel,
    int AvailableDaysPerWeek,
    TrainingExperienceLevel TrainingExperienceLevel
) : IRequest<RequestResponse<bool>>;

}
