namespace IncidentManager.Domain.Incidents.ValueObjects;

public enum IncidentStatus
{
    Open         = 0,
    Acknowledged = 1,
    InProgress   = 2,
    Resolved     = 3,
    Closed       = 4
}

public static class IncidentStatusExtensions
{
    // Máquina de estados: define qué transiciones son válidas.
    // Closed es un estado terminal — no se puede reabrir.
    private static readonly Dictionary<IncidentStatus, HashSet<IncidentStatus>> ValidTransitions = new()
    {
        [IncidentStatus.Open]         = [IncidentStatus.Acknowledged, IncidentStatus.InProgress],
        [IncidentStatus.Acknowledged] = [IncidentStatus.InProgress],
        [IncidentStatus.InProgress]   = [IncidentStatus.Resolved],
        [IncidentStatus.Resolved]     = [IncidentStatus.Closed, IncidentStatus.InProgress], // puede reabrir
        [IncidentStatus.Closed]       = []
    };

    public static bool CanTransitionTo(this IncidentStatus current, IncidentStatus next)
        => ValidTransitions.TryGetValue(current, out var allowed) && allowed.Contains(next);
}
