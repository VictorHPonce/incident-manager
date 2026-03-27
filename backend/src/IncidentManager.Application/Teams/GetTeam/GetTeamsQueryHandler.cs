using MediatR;
using IncidentManager.Application.Teams.DTOs;
using IncidentManager.Domain.Auth.Ports;
using IncidentManager.Domain.Shared;
using IncidentManager.Domain.Teams.Ports;

namespace IncidentManager.Application.Teams.GetTeam;

internal sealed class GetTeamsQueryHandler(
    ITeamRepository teamRepository,
    ICurrentUser currentUser)
    : IRequestHandler<GetTeamsQuery, Result<IReadOnlyList<TeamResponse>>>
{
    public async Task<Result<IReadOnlyList<TeamResponse>>> Handle(
        GetTeamsQuery query, CancellationToken ct)
    {
        if (currentUser.IsAdmin)
        {
            var all = await teamRepository.GetAllActiveAsync(ct);
            return Result<IReadOnlyList<TeamResponse>>.Success(
                all.Select(t => new TeamResponse(
                    t.Id, t.Name, t.Slug, t.IsActive, t.CreatedAt))
                   .ToList());
        }

        var team = await teamRepository.GetByIdAsync(currentUser.TeamId, ct);
        if (team is null)
            return Result<IReadOnlyList<TeamResponse>>.Failure("Equipo no encontrado.");

        return Result<IReadOnlyList<TeamResponse>>.Success(
            new List<TeamResponse> {
                new(team.Id, team.Name, team.Slug, team.IsActive, team.CreatedAt)
            });
    }
}