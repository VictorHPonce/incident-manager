using IncidentManager.Domain.Incidents.ValueObjects;

namespace IncidentManager.Application.Incidents.DTOs;

/// <summary>
/// DTO ligero para listados — sin campos pesados como SourceMetadata.
/// </summary>
public record IncidentListItem(
    Guid Id,
    string Title,
    Severity Severity,
    IncidentStatus Status,
    SourceType SourceType,
    Guid TeamId,
    Guid? AssignedTo,
    DateTimeOffset CreatedAt,
    DateTimeOffset SlaDeadline,
    bool IsSlaBreached,
    string[] Tags
);