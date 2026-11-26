namespace FitnessProfileService.Messaging.Exceptions
{
    public sealed class InvalidMessageException(string message)
    : Exception(message);

    public sealed class BusinessException(string message)
    : Exception(message);

    public sealed class ConsumerProcessingException(string message)
    : Exception(message);
}
