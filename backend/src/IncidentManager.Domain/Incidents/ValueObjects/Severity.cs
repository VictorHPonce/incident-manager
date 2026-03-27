namespace IncidentManager.Domain.Incidents.ValueObjects;

public enum Severity { Low = 0, Medium = 1, High = 2, Critical = 3 }

public static class SeverityExtensions
{
    /// <summary>Horas máximas para resolver la incidencia según SLA.</summary>
    public static int SlaHours(this Severity s) => s switch
    {
        Severity.Critical => 1,
        Severity.High     => 4,
        Severity.Medium   => 24,
        Severity.Low      => 72,
        _                 => throw new ArgumentOutOfRangeException(nameof(s))
    };
}
