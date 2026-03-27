using IncidentManager.Application.Common.Interfaces;
using IncidentManager.Application.Incidents.DTOs;
using IncidentManager.Domain.Incidents.ValueObjects;
using IncidentManager.Domain.Shared;

namespace IncidentManager.Application.Incidents.ListIncidents;

public record ListIncidentsQuery(
    IncidentStatus? Status = null,
    Severity? Severity = null,
    string? Search = null,   // busca en título
    int Page = 1,
    int PageSize = 20
) : IQuery<PagedResult<IncidentListItem>>;