using FitnessProfileService.Common.Responses;
using MediatR;

namespace FitnessProfileService.Features.CreateFitnessProfile.Queries
{
    public record CheckFitnessProfileExistsQuery(Guid UserId)
     : IRequest<RequestResponse<bool>>;
}
