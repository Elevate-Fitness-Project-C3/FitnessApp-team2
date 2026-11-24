using FitnessProfileService.Common.Responses;
using MediatR;
using static MassTransit.ValidationResultExtensions;

namespace FitnessProfileService.FitnessProfileService.Features.CreateFitnessProfile.Commands
{
    public record CreateFitnessProfileCommand(Guid UserId) : IRequest<RequestResponse<bool>>;
}
