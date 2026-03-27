using MediatR;
using IncidentManager.Application.Incidents.DTOs;
using IncidentManager.Application.Incidents.CreateIncident;
using IncidentManager.Domain.Incidents.Ports;
using IncidentManager.Domain.Shared;

namespace IncidentManager.Application.Incidents.GetIncident;

internal sealed class GetIncidentQueryHandler(IIncidentRepository repository)
    : IRequestHandler<GetIncidentQuery, Result<IncidentResponse>>
{
    public async Task<Result<IncidentResponse>> Handle(GetIncidentQuery query, CancellationToken ct)
    {
        var incident = await repository.GetByIdAsync(query.Id, ct);
        return incident is null
            ? Result<IncidentResponse>.Failure($"Incidencia '{query.Id}' no encontrada.")
            : Result<IncidentResponse>.Success(CreateIncidentCommandHandler.Map(incident));
    }
}
