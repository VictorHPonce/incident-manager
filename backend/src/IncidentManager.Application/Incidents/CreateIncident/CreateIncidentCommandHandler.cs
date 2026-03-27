using MediatR;
using IncidentManager.Application.Incidents.DTOs;
using IncidentManager.Domain.Incidents.Entities;
using IncidentManager.Domain.Incidents.Ports;
using IncidentManager.Domain.Shared;

namespace IncidentManager.Application.Incidents.CreateIncident;

internal sealed class CreateIncidentCommandHandler(IIncidentRepository repository, IIncidentMetrics metrics)
    : IRequestHandler<CreateIncidentCommand, Result<IncidentResponse>>
{
    public async Task<Result<IncidentResponse>> Handle(
        CreateIncidentCommand cmd, CancellationToken ct)
    {
        var incident = Incident.Create(
            cmd.Title, cmd.Description, cmd.Severity,
            cmd.SourceType, cmd.TeamId, cmd.CreatedBy,
            cmd.SourceRef, cmd.Tags);

        await repository.AddAsync(incident, ct);
        // Métrica — sin violar la arquitectura
        metrics.IncidentCreated(
            incident.Severity.ToString(),
            incident.SourceType.ToString());
        return Result<IncidentResponse>.Success(Map(incident));
    }

    internal static IncidentResponse Map(Incident i) => new(
        i.Id, i.Title, i.Description, i.Severity, i.Status,
        i.SourceType, i.SourceRef, i.TeamId, i.CreatedBy, i.AssignedTo,
        i.CreatedAt, i.UpdatedAt, i.SlaDeadline, i.IsSlaBreached(), i.Tags);
}
