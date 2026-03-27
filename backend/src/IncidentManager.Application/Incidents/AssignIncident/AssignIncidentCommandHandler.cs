using MediatR;
using IncidentManager.Domain.Auth.Ports;
using IncidentManager.Domain.Incidents.Ports;
using IncidentManager.Domain.Shared;

namespace IncidentManager.Application.Incidents.AssignIncident;

internal sealed class AssignIncidentCommandHandler(
    IIncidentRepository incidentRepository,
    ICurrentUser currentUser)
    : IRequestHandler<AssignIncidentCommand, Result>
{
    public async Task<Result> Handle(
        AssignIncidentCommand cmd, CancellationToken ct)
    {
        if (!currentUser.IsTeamLead)
            return Result.Failure("Solo TeamLead o Admin pueden asignar incidencias.");

        var incident = await incidentRepository.GetByIdAsync(cmd.IncidentId, ct);
        if (incident is null)
            return Result.Failure($"Incidencia '{cmd.IncidentId}' no encontrada.");

        if (!currentUser.IsAdmin && incident.TeamId != currentUser.TeamId)
            return Result.Failure("No puedes asignar incidencias de otro equipo.");

        incident.AssignTo(cmd.AssignedTo);
        await incidentRepository.UpdateAsync(incident, ct);

        return Result.Success();
    }
}