using IdentityService.Features.Register.Commands;
using System.Net.Mail;

namespace IdentityService.Features.Register.Validators;

public static class RegisterValidator
{
    public const int MinimumPasswordLength = 15;
    public const int MaximumPasswordLength = 128;

    public static Dictionary<string, string[]> Validate(RegisterCommand request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Trim().Length > 200)
            errors["name"] = ["Enter a name between 1 and 200 characters."];

        var email = request.Email?.Trim();
        if (string.IsNullOrWhiteSpace(email) || email.Length > 256 ||
            !MailAddress.TryCreate(email, out var address) ||
            address.DisplayName.Length > 0 || !string.Equals(address.Address, email, StringComparison.Ordinal))
            errors["email"] = ["Enter a valid email address of up to 256 characters."];

        if (string.IsNullOrWhiteSpace(request.Password) ||
            request.Password.Length < MinimumPasswordLength ||
            request.Password.Length > MaximumPasswordLength)
            errors["password"] = ["Use a password between 15 and 128 characters."];

        return errors;
    }
}
