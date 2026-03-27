using IncidentManager.Application.Common.Interfaces;
using IncidentManager.Domain.Incidents.ValueObjects;

namespace IncidentManager.Application.Incidents.UpdateIncidentStatus;

public record UpdateIncidentStatusCommand(
    Guid           IncidentId,
    IncidentStatus NewStatus,
    Guid           UpdatedBy
) : ICommand;
