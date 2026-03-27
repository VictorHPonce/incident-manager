namespace IncidentManager.Application.Teams.DTOs;

public record TeamMemberResponse(
    Guid Id,
    string Email,
    string DisplayName,
    string Role,
    bool IsActive
);