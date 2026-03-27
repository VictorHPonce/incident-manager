using MediatR;
using IncidentManager.Application.Incidents.DTOs;
using IncidentManager.Domain.Auth.Ports;
using IncidentManager.Domain.Incidents.Entities;
using IncidentManager.Domain.Incidents.Ports;
using IncidentManager.Domain.Shared;

namespace IncidentManager.Application.Incidents.ListIncidents;

internal sealed class ListIncidentsQueryHandler(
    IIncidentRepository repository,
    ICurrentUser currentUser)
    : IRequestHandler<ListIncidentsQuery, Result<PagedResult<IncidentListItem>>>
{
    public async Task<Result<PagedResult<IncidentListItem>>> Handle(
        ListIncidentsQuery query, CancellationToken ct)
    {
        // RLS: Admin ve todo, el resto solo su equipo
        var teamId = currentUser.IsAdmin ? (Guid?)null : currentUser.TeamId;

        var paged = await repository.GetFilteredAsync(
            teamId: teamId,
            status: query.Status,
            severity: query.Severity,
            search: query.Search,
            page: query.Page,
            pageSize: query.PageSize,
            ct: ct);

        var items = paged.Items
            .Select(Map)
            .ToList();

        return Result<PagedResult<IncidentListItem>>.Success(
            new PagedResult<IncidentListItem>(items, paged.Page, paged.PageSize, paged.TotalCount));
    }

    private static IncidentListItem Map(Incident i) => new(
        i.Id, i.Title, i.Severity, i.Status, i.SourceType,
        i.TeamId, i.AssignedTo, i.CreatedAt, i.SlaDeadline,
        i.IsSlaBreached(), i.Tags);
}