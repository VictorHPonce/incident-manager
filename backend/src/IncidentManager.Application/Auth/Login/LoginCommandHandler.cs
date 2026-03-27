using MediatR;
using IncidentManager.Application.Auth.DTOs;
using IncidentManager.Domain.Auth.Ports;
using IncidentManager.Domain.Shared;
using IncidentManager.Domain.Teams.Ports;

namespace IncidentManager.Application.Auth.Login;

internal sealed class LoginCommandHandler(
    IUserRepository userRepository,
    ITokenService tokenService,
    IPasswordHasher passwordHasher)
    : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(
        LoginCommand cmd, CancellationToken ct)
    {
        // 1. Buscar usuario por email
        var user = await userRepository.GetByEmailAsync(cmd.Email, ct);

        // 2. Mismo mensaje para "no existe" y "contraseña incorrecta"
        //    Nunca reveles cuál de los dos falló — es info para un atacante
        if (user is null || !user.IsActive || !passwordHasher.Verify(cmd.Password, user.PasswordHash))
            return Result<AuthResponse>.Failure("Credenciales inválidas.");

        // 3. Generar par de tokens
        var tokens = tokenService.Generate(
            user.Id, user.Email, user.Role.ToString(), user.TeamId);

        // 4. Persistir refresh token en Redis con TTL
        await tokenService.StoreRefreshTokenAsync(tokens.RefreshToken, user.Id, ct);

        // 5. Devolver LoginResponse con accessToken + refreshToken
        //    El endpoint decide qué va al body y qué va a la cookie
        return Result<AuthResponse>.Success(new AuthResponse(
            AccessToken: tokens.AccessToken,
            RefreshToken: tokens.RefreshToken,
            ExpiresAt: DateTimeOffset.UtcNow.AddMinutes(15),
            Email: user.Email,
            Role: user.Role.ToString(),
            TeamId: user.TeamId));
    }
}