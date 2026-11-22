using System.Text.Json.Serialization;
using MediatR;

namespace IdentityService.Features.Register.Notifications;

public sealed class SendVerificationEmailNotification(string email, string code, DateTime expiresAtUtc) : INotification
{
    public string Email { get; } = email;
    [JsonIgnore]
    public string Code { get; } = code;
    public DateTime ExpiresAtUtc { get; } = expiresAtUtc;
}
