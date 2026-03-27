using MediatR;
using IncidentManager.Domain.Incidents.Ports;
using IncidentManager.Domain.Shared;

namespace IncidentManager.Application.Incidents.UpdateIncidentStatus;

internal sealed class UpdateIncidentStatusCommandHandler(IIncidentRepository repository)
    : IRequestHandler<UpdateIncidentStatusCommand, Result>
{
    public async Task<Result> Handle(UpdateIncidentStatusCommand cmd, CancellationToken ct)
    {
        var incident = await repository.GetByIdAsync(cmd.IncidentId, ct);
        if (incident is null)
            return Result.Failure($"Incidencia '{cmd.IncidentId}' no encontrada.");

        incident.ChangeStatus(cmd.NewStatus);
        await repository.UpdateAsync(incident, ct);
        return Result.Success();
    }
}
