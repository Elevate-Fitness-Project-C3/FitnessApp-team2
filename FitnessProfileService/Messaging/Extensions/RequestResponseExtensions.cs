using FitnessProfileService.Common.Responses;
using FitnessProfileService.Messaging.Exceptions;

namespace FitnessProfileService.Messaging.Extensions
{
    public static class RequestResponseExtensions
    {
        public static void ThrowIfFailed(this RequestResponse response)
        {
            if (response.Success)
                return;

            throw response.StatusCode switch
            {
                400 => new InvalidMessageException(response.Message),
                404 => new BusinessException(response.Message),
                409 => new BusinessException(response.Message),

                _ => new ConsumerProcessingException(response.Message)
            };
        }
    }
}