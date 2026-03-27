using MediatR;
using IncidentManager.Application.Teams.CreateTeam;
using IncidentManager.Application.Teams.DTOs;
using IncidentManager.Application.Teams.GetTeam;

namespace IncidentManager.API.Endpoints.Teams;

public static class TeamEndpoints
{
    public static IEndpointRouteBuilder MapTeamEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/teams")
                   .WithTags("Teams")
                   .RequireAuthorization();

        // GET /api/teams — Admin ve todos, TeamLead/Técnico ve solo el suyo
        g.MapGet("/", async (IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new GetTeamsQuery(), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(new { error = result.Error });
        })
        .WithName("GetTeams")
        .WithSummary("Lista equipos activos.")
        .Produces<IReadOnlyList<TeamResponse>>();

        // POST /api/teams — Solo Admin
        g.MapPost("/", async (CreateTeamCommand cmd, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(cmd, ct);
            return result.IsSuccess
                ? Results.Created($"/api/teams/{result.Value!.Id}", result.Value)
                : Results.BadRequest(new { error = result.Error });
        })
        .WithName("CreateTeam")
        .WithSummary("Crea un equipo. Solo Admin.")
        .Produces<TeamResponse>();

        // GET /api/teams/{id}/members — Admin y TeamLead del equipo
        g.MapGet("/{id:guid}/members", async (
            Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new GetTeamMembersQuery(id), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(new { error = result.Error });
        })
        .WithName("GetTeamMembers")
        .WithSummary("Lista miembros del equipo.")
        .Produces<IReadOnlyList<TeamMemberResponse>>();

        return app;
    }
}