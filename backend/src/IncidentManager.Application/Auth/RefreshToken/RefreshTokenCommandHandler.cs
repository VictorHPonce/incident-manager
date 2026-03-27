using MediatR;
using IncidentManager.Application.Auth.DTOs;
using IncidentManager.Domain.Auth.Ports;
using IncidentManager.Domain.Shared;
using IncidentManager.Domain.Teams.Ports;

namespace IncidentManager.Application.Auth.RefreshToken;

internal sealed class RefreshTokenCommandHandler(
    ITokenService tokenService,
    IUserRepository userRepository)
    : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(
        RefreshTokenCommand cmd, CancellationToken ct)
    {
        var userId = await tokenService.GetUserIdFromRefreshTokenAsync(cmd.RefreshToken, ct);
        if (userId is null)
            return Result<AuthResponse>.Failure("Refresh token inválido o expirado.");

        var user = await userRepository.GetByIdAsync(userId.Value, ct);
        if (user is null || !user.IsActive)
            return Result<AuthResponse>.Failure("Usuario no encontrado o inactivo.");

        await tokenService.RevokeRefreshTokenAsync(cmd.RefreshToken, ct);
        var tokens = tokenService.Generate(
            user.Id, user.Email, user.Role.ToString(), user.TeamId);
        await tokenService.StoreRefreshTokenAsync(tokens.RefreshToken, user.Id, ct);

        return Result<AuthResponse>.Success(new AuthResponse(
            tokens.AccessToken,
            tokens.RefreshToken,   // ← ahora sí lo devuelve
            DateTimeOffset.UtcNow.AddMinutes(15),
            user.Email,
            user.Role.ToString(),
            user.TeamId));
    }
}