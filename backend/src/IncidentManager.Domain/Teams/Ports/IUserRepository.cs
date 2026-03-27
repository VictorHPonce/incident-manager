using IncidentManager.Domain.Teams.Entities;

namespace IncidentManager.Domain.Teams.Ports;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task        AddAsync(User user, CancellationToken ct = default);
    Task        UpdateAsync(User user, CancellationToken ct = default);
    Task<IReadOnlyList<User>> GetByTeamAsync(Guid teamId, CancellationToken ct = default);
}
