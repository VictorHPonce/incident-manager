using MediatR;
using IncidentManager.Application.Teams.DTOs;
using IncidentManager.Domain.Auth.Ports;
using IncidentManager.Domain.Shared;
using IncidentManager.Domain.Teams.Entities;
using IncidentManager.Domain.Teams.Ports;

namespace IncidentManager.Application.Teams.CreateTeam;

internal sealed class CreateTeamCommandHandler(
    ITeamRepository teamRepository,
    ICurrentUser currentUser)
    : IRequestHandler<CreateTeamCommand, Result<TeamResponse>>
{
    public async Task<Result<TeamResponse>> Handle(
        CreateTeamCommand cmd, CancellationToken ct)
    {
        // Autorización en el Handler — no depende del endpoint
        if (!currentUser.IsAdmin)
            return Result<TeamResponse>.Failure("Solo los administradores pueden crear equipos.");

        var team = Team.Create(cmd.Name);
        await teamRepository.AddAsync(team, ct);

        return Result<TeamResponse>.Success(
            new TeamResponse(team.Id, team.Name, team.Slug, team.IsActive, team.CreatedAt));
    }
}