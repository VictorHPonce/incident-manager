using Microsoft.EntityFrameworkCore;
using IncidentManager.Domain.Incidents.Entities;
using IncidentManager.Domain.Incidents.Ports;
using IncidentManager.Domain.Incidents.ValueObjects;
using IncidentManager.Domain.Shared;
using IncidentManager.Infrastructure.Persistence;

namespace IncidentManager.Infrastructure.Persistence.Repositories.Incidents;

internal sealed class IncidentRepository(IncidentManagerDbContext db) : IIncidentRepository
{
    public async Task<Incident?> GetByIdAsync(Guid id, CancellationToken ct)
        => await db.Incidents.FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task<PagedResult<Incident>> GetByTeamAsync(
        Guid teamId, int page, int pageSize, CancellationToken ct)
    {
        var q     = db.Incidents.Where(i => i.TeamId == teamId).OrderByDescending(i => i.CreatedAt);
        var total = await q.CountAsync(ct);
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<Incident>(items, page, pageSize, total);
    }

    public async Task<Incident?> GetBySourceRefAsync(string sourceRef, CancellationToken ct)
        => await db.Incidents.FirstOrDefaultAsync(i => i.SourceRef == sourceRef, ct);

    public async Task<IReadOnlyList<Incident>> GetBreachedSlaAsync(CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        return await db.Incidents
            .Where(i => i.SlaDeadline < now
                     && i.Status != IncidentStatus.Resolved
                     && i.Status != IncidentStatus.Closed)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Incident incident, CancellationToken ct)
    {
        await db.Incidents.AddAsync(incident, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Incident incident, CancellationToken ct)
    {
        db.Incidents.Update(incident);
        await db.SaveChangesAsync(ct);
    }

    public async Task<PagedResult<Incident>> GetFilteredAsync(
        Guid? teamId,
        IncidentStatus? status,
        Severity? severity,
        string? search,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var q = db.Incidents.AsQueryable();

        // Filtro por equipo — null significa Admin (ve todo)
        if (teamId.HasValue)
            q = q.Where(i => i.TeamId == teamId.Value);

        if (status.HasValue)
            q = q.Where(i => i.Status == status.Value);

        if (severity.HasValue)
            q = q.Where(i => i.Severity == severity.Value);

        // Búsqueda simple en título — full-text search viene en MVP-3
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(i => i.Title.ToLower().Contains(search.ToLower()));

        q = q.OrderByDescending(i => i.CreatedAt);

        var total = await q.CountAsync(ct);
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        return new PagedResult<Incident>(items, page, pageSize, total);
    }
}
