using System.Text.Json.Serialization;

namespace IdentityService.Features.Register.DTOs;

public sealed class CreatedVerificationCodeDto
{
    [JsonIgnore]
    public required string Code { get; init; }
    public DateTime ExpiresAtUtc { get; init; }
}
