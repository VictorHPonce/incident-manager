namespace IncidentManager.Application.Teams.DTOs;

public record TeamResponse(Guid Id, string Name, string Slug, bool IsActive, DateTimeOffset CreatedAt);
