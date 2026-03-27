namespace IncidentManager.Domain.Incidents.Exceptions;

public sealed class IncidentNotFoundException(Guid id)
    : Exception($"Incidencia '{id}' no encontrada.");
