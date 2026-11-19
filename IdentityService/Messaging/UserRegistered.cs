namespace Fitness.Contracts.Identity.V1;

public sealed record UserRegistered(Guid EventId, string UserId, DateTimeOffset OccurredAtUtc);
