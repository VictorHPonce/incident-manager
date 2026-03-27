using IncidentManager.Application.Common.Interfaces;
using IncidentManager.Application.Teams.DTOs;

namespace IncidentManager.Application.Teams.CreateTeam;

public record CreateTeamCommand(string Name) : ICommand<TeamResponse>;