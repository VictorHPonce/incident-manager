using Microsoft.EntityFrameworkCore;
using IncidentManager.Domain.Teams.Entities;
using IncidentManager.Domain.Teams.Ports;
using IncidentManager.Infrastructure.Persistence;

namespace IncidentManager.Infrastructure.Persistence.Repositories.Auth;

internal sealed class UserRepository(IncidentManagerDbContext db) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
        => await db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
        => await db.Users.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), ct);

    public async Task AddAsync(User user, CancellationToken ct)
    {
        await db.Users.AddAsync(user, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(User user, CancellationToken ct)
    {
        db.Users.Update(user);
        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<User>> GetByTeamAsync(Guid teamId, CancellationToken ct)
    => await db.Users
               .Where(u => u.TeamId == teamId && u.IsActive)
               .OrderBy(u => u.DisplayName)
               .ToListAsync(ct);
}
