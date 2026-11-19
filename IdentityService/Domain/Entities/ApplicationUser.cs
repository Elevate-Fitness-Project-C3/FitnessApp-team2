using Microsoft.AspNetCore.Identity;

namespace IdentityService.Domain.Entities;

public sealed class ApplicationUser : IdentityUser
{
    public required string FullName { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
