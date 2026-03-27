using IncidentManager.Domain.Teams.Entities;

namespace IncidentManager.Domain.Teams.Ports;

public interface ITeamRepository
{
    Task<Team?>               GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Team>> GetAllActiveAsync(CancellationToken ct = default);
    Task                      AddAsync(Team team, CancellationToken ct = default);
    Task                      UpdateAsync(Team team, CancellationToken ct = default);
}
