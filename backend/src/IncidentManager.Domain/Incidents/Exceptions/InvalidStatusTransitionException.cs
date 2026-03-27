using IncidentManager.Domain.Incidents.ValueObjects;

namespace IncidentManager.Domain.Incidents.Exceptions;

public sealed class InvalidStatusTransitionException(IncidentStatus from, IncidentStatus to)
    : Exception($"Transición inválida: '{from}' → '{to}'.");
