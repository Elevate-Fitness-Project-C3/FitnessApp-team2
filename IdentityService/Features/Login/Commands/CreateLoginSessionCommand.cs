using IdentityService.Domain.Entities;
using IdentityService.Features.Login.ViewModels;
using MediatR;

namespace IdentityService.Features.Login.Commands;

public sealed record CreateLoginSessionCommand(ApplicationUser User) : IRequest<LoginSessionViewModel>
{
    public sealed class Handler(LoginSessions sessions)
        : IRequestHandler<CreateLoginSessionCommand, LoginSessionViewModel>
    {
        public Task<LoginSessionViewModel> Handle(CreateLoginSessionCommand command, CancellationToken cancellationToken) =>
            sessions.CreateSessionAsync(command.User, cancellationToken);
    }
}
