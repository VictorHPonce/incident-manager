using MediatR;
using IncidentManager.Application.Teams.DTOs;
using IncidentManager.Domain.Auth.Ports;
using IncidentManager.Domain.Shared;
using IncidentManager.Domain.Teams.Ports;

namespace IncidentManager.Application.Teams.GetTeam;

internal sealed class GetTeamMembersQueryHandler(
    IUserRepository userRepository,
    ICurrentUser currentUser)
    : IRequestHandler<GetTeamMembersQuery, Result<IReadOnlyList<TeamMemberResponse>>>
{
    public async Task<Result<IReadOnlyList<TeamMemberResponse>>> Handle(
        GetTeamMembersQuery query, CancellationToken ct)
    {
        // TeamLead solo puede ver miembros de su propio equipo
        if (!currentUser.IsAdmin && currentUser.TeamId != query.TeamId)
            return Result<IReadOnlyList<TeamMemberResponse>>
                .Failure("No tienes permisos para ver este equipo.");

        var members = await userRepository.GetByTeamAsync(query.TeamId, ct);

        return Result<IReadOnlyList<TeamMemberResponse>>.Success(
            members.Select(u => new TeamMemberResponse(
                u.Id, u.Email, u.DisplayName, u.Role.ToString(), u.IsActive))
                   .ToList());
    }
}