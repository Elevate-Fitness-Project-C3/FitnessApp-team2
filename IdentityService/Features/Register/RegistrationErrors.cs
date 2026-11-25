using IdentityService.Common.ResultPattern;

namespace IdentityService.Features.Register;

internal static class RegistrationErrors
{
    public static RequestResponse<T> EmailAlreadyRegistered<T>() =>
        RequestResponse<T>.Fail("Registration failed.", StatusCodes.Status409Conflict,
            new Dictionary<string, string[]> { ["email"] = ["This email is already registered."] },
            errorCode: "email_already_registered");
}
