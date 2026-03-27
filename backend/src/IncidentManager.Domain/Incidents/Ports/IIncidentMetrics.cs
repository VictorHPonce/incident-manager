namespace IncidentManager.Domain.Incidents.Ports;

public interface IIncidentMetrics
{
    void IncidentCreated(string severity, string sourceType);
    void SlaBreached(string severity);
}