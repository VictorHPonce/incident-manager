namespace IncidentManager.Domain.Incidents.ValueObjects;

/// <summary>Origen de la incidencia. Determina cómo se parsean los metadatos.</summary>
public enum SourceType
{
    Manual     = 0,  // Creada por un usuario
    Prometheus = 1,  // Webhook de Prometheus Alertmanager
    Gitea      = 2,  // Pipeline fallida en Gitea Runner
    External   = 3   // Otros sistemas via API
}
