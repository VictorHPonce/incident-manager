namespace IncidentManager.Application.Auth.DTOs;

public record LoginResult(
    string AccessToken,
    DateTimeOffset ExpiresAt,
    string Email,
    string Role,
    Guid TeamId
);