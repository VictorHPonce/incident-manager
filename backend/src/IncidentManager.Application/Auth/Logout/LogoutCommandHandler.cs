using MediatR;
using IncidentManager.Domain.Auth.Ports;
using IncidentManager.Domain.Shared;

namespace IncidentManager.Application.Auth.Logout;

internal sealed class LogoutCommandHandler(ITokenService tokenService)
    : IRequestHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(LogoutCommand cmd, CancellationToken ct)
    {
        await tokenService.RevokeRefreshTokenAsync(cmd.RefreshToken, ct);
        return Result.Success();
    }
}