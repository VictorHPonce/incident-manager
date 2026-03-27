using IncidentManager.Application.Common.Interfaces;

namespace IncidentManager.Application.Incidents.AssignIncident;

public record AssignIncidentCommand(
    Guid IncidentId,
    Guid AssignedTo
) : ICommand;