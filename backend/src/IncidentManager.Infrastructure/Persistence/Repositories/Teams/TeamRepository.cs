using Microsoft.EntityFrameworkCore;
using IncidentManager.Domain.Teams.Entities;
using IncidentManager.Domain.Teams.Ports;
using IncidentManager.Infrastructure.Persistence;

namespace IncidentManager.Infrastructure.Persistence.Repositories.Teams;

internal sealed class TeamRepository(IncidentManagerDbContext db) : ITeamRepository
{
    public async Task<Team?> GetByIdAsync(Guid id, CancellationToken ct)
        => await db.Teams.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<IReadOnlyList<Team>> GetAllActiveAsync(CancellationToken ct)
        => await db.Teams.Where(t => t.IsActive).ToListAsync(ct);

    public async Task AddAsync(Team team, CancellationToken ct)
    {
        await db.Teams.AddAsync(team, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Team team, CancellationToken ct)
    {
        db.Teams.Update(team);
        await db.SaveChangesAsync(ct);
    }
}
