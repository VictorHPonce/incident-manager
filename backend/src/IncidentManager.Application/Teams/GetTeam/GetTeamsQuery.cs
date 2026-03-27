using IncidentManager.Application.Common.Interfaces;
using IncidentManager.Application.Teams.DTOs;

namespace IncidentManager.Application.Teams.GetTeam;

public record GetTeamsQuery : IQuery<IReadOnlyList<TeamResponse>>;