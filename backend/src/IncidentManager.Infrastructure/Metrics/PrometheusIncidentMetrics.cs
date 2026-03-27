using IncidentManager.Domain.Incidents.Ports;
using Prometheus;

namespace IncidentManager.Infrastructure.Metrics;

internal sealed class PrometheusIncidentMetrics : IIncidentMetrics
{
    private static readonly Counter Created = global::Prometheus.Metrics.CreateCounter(
        "im_incidents_created_total",
        "Total de incidencias creadas",
        new CounterConfiguration { LabelNames = ["severity", "source_type"] });

    private static readonly Counter Breaches = global::Prometheus.Metrics.CreateCounter(
        "im_sla_breaches_total",
        "Total de incidencias que superaron el SLA",
        new CounterConfiguration { LabelNames = ["severity"] });

    public void IncidentCreated(string severity, string sourceType)
        => Created.WithLabels(severity, sourceType).Inc();

    public void SlaBreached(string severity)
        => Breaches.WithLabels(severity).Inc();
}