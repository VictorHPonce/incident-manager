namespace IncidentManager.Domain.Auth.Ports;

public record TokenPair(string AccessToken, string RefreshToken);

public interface ITokenService
{
    TokenPair Generate(Guid userId, string email, string role, Guid teamId);
    Guid?     ValidateRefreshToken(string refreshToken);
    Task      RevokeRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    Task      StoreRefreshTokenAsync(string refreshToken, Guid userId, CancellationToken ct = default);
    Task<Guid?> GetUserIdFromRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
}
