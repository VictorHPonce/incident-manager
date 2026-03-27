using IncidentManager.Application.Common.Interfaces;
using IncidentManager.Application.Incidents.DTOs;
using IncidentManager.Domain.Incidents.ValueObjects;

namespace IncidentManager.Application.Incidents.CreateIncident;

public record CreateIncidentCommand(
    string Title,
    string? Description,
    Severity Severity,
    SourceType SourceType,
    Guid TeamId,
    Guid CreatedBy,
    string? SourceRef = null,
    string[]? Tags = null
) : ICommand<IncidentResponse>;
