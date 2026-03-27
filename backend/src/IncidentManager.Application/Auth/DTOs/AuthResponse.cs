namespace IncidentManager.Application.Auth.DTOs;

/// <summary>
/// Respuesta interna de los handlers de Auth.
/// El endpoint decide qué va al body (AccessToken) y qué va a cookie (RefreshToken).
/// </summary>
public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset ExpiresAt,
    string Email,
    string Role,
    Guid TeamId
);