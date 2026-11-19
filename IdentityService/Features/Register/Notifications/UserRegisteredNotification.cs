using MediatR;

namespace IdentityService.Features.Register.Notifications;

public sealed record UserRegisteredNotification(string UserId) : INotification;
