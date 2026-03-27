using IncidentManager.Domain.Incidents.Entities;
using IncidentManager.Domain.Incidents.ValueObjects;
using IncidentManager.Domain.Shared;

namespace IncidentManager.Domain.Incidents.Ports;

/// <summary>
/// Puerto de salida del módulo Incidents.
/// Infrastructure lo implementa con EF Core + PostgreSQL.
/// Los tests lo implementan con repositorios en memoria.
/// </summary>
public interface IIncidentRepository
{
    Task<Incident?>               GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<Incident>>   GetByTeamAsync(Guid teamId, int page, int pageSize, CancellationToken ct = default);
    Task<Incident?>               GetBySourceRefAsync(string sourceRef, CancellationToken ct = default);
    Task<IReadOnlyList<Incident>> GetBreachedSlaAsync(CancellationToken ct = default);
    Task                          AddAsync(Incident incident, CancellationToken ct = default);
    Task                          UpdateAsync(Incident incident, CancellationToken ct = default);
    Task<PagedResult<Incident>> GetFilteredAsync(
    Guid? teamId,
    IncidentStatus? status,
    Severity? severity,
    string? search,
    int page,
    int pageSize,
    CancellationToken ct = default);
}
