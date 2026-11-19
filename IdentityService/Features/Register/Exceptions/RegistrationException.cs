namespace IdentityService.Features.Register.Exceptions;

public sealed class RegistrationException(string message, int statusCode, string errorCode) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public string ErrorCode { get; } = errorCode;
}
