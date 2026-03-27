using IncidentManager.Domain.Incidents.ValueObjects;

namespace IncidentManager.Application.Incidents.DTOs;

/// <summary>
/// DTO de salida para incidencias.
/// El endpoint recibe esto — nunca la entidad Domain directamente.
/// </summary>
public record IncidentResponse(
    Guid           Id,
    string         Title,
    string?        Description,
    Severity       Severity,
    IncidentStatus Status,
    SourceType     SourceType,
    string?        SourceRef,
    Guid           TeamId,
    Guid           CreatedBy,
    Guid?          AssignedTo,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset SlaDeadline,
    bool           IsSlaBreached,
    string[]       Tags
);
