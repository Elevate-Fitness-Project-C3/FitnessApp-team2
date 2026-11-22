using Microsoft.AspNetCore.Identity;

namespace IdentityService.Domain.Entities;

public sealed class ApplicationUser : IdentityUser
{
    public required string FullName { get; set; }
    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; }
}
