using IncidentManager.Domain.Incidents.Exceptions;
using IncidentManager.Domain.Incidents.ValueObjects;

namespace IncidentManager.Domain.Incidents.Entities;

/// <summary>
/// Entidad raíz del módulo de incidencias.
/// Toda la lógica de negocio vive aquí, no en los servicios.
/// Constructor privado → usar Create() para construir una instancia válida.
/// </summary>
public sealed class Incident
{
    public Guid             Id             { get; private set; }
    public string           Title          { get; private set; } = string.Empty;
    public string?          Description    { get; private set; }
    public Severity         Severity       { get; private set; }
    public IncidentStatus   Status         { get; private set; }
    public SourceType       SourceType     { get; private set; }
    public string?          SourceRef      { get; private set; }
    public Guid             TeamId         { get; private set; }
    public Guid             CreatedBy      { get; private set; }
    public Guid?            AssignedTo     { get; private set; }
    public DateTimeOffset   CreatedAt      { get; private set; }
    public DateTimeOffset   UpdatedAt      { get; private set; }
    public DateTimeOffset   SlaDeadline    { get; private set; }
    public string[]         Tags           { get; private set; } = [];
    public Dictionary<string, object> SourceMetadata { get; private set; } = [];

    private Incident() { } // EF Core necesita constructor sin parámetros

    /// <summary>
    /// Único punto de creación válido.
    /// Calcula el SlaDeadline según la severidad.
    /// </summary>
    public static Incident Create(
        string     title,
        string?    description,
        Severity   severity,
        SourceType sourceType,
        Guid       teamId,
        Guid       createdBy,
        string?    sourceRef  = null,
        string[]?  tags       = null,
        Dictionary<string, object>? sourceMetadata = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("El título es obligatorio.", nameof(title));

        var now = DateTimeOffset.UtcNow;
        return new Incident
        {
            Id             = Guid.NewGuid(),
            Title          = title.Trim(),
            Description    = description?.Trim(),
            Severity       = severity,
            Status         = IncidentStatus.Open,
            SourceType     = sourceType,
            SourceRef      = sourceRef,
            TeamId         = teamId,
            CreatedBy      = createdBy,
            CreatedAt      = now,
            UpdatedAt      = now,
            SlaDeadline    = now.AddHours(severity.SlaHours()),
            Tags           = tags ?? [],
            SourceMetadata = sourceMetadata ?? []
        };
    }

    /// <summary>Cambia el estado respetando la máquina de estados.</summary>
    public void ChangeStatus(IncidentStatus newStatus)
    {
        if (!Status.CanTransitionTo(newStatus))
            throw new InvalidStatusTransitionException(Status, newStatus);
        Status    = newStatus;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void AssignTo(Guid userId) { AssignedTo = userId; UpdatedAt = DateTimeOffset.UtcNow; }

    /// <summary>True si ha expirado el SLA y la incidencia no está resuelta.</summary>
    public bool IsSlaBreached()
        => Status is not (IncidentStatus.Resolved or IncidentStatus.Closed)
        && DateTimeOffset.UtcNow > SlaDeadline;
}
