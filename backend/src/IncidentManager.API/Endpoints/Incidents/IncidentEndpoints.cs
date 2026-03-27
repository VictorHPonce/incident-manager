using MediatR;
using Microsoft.AspNetCore.Mvc;
using IncidentManager.Application.Incidents.AssignIncident;
using IncidentManager.Application.Incidents.CreateIncident;
using IncidentManager.Application.Incidents.DTOs;
using IncidentManager.Application.Incidents.GetIncident;
using IncidentManager.Application.Incidents.ListIncidents;
using IncidentManager.Application.Incidents.UpdateIncidentStatus;
using IncidentManager.Domain.Incidents.ValueObjects;
using IncidentManager.Domain.Shared;

namespace IncidentManager.API.Endpoints.Incidents;

public static class IncidentEndpoints
{
    public static IEndpointRouteBuilder MapIncidentEndpoints(
        this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/incidents")
                   .WithTags("Incidents")
                   .RequireAuthorization();

        // GET /api/incidents?status=Open&severity=High&search=cpu&page=1&pageSize=20
        g.MapGet("/", async (
            [FromQuery] IncidentStatus? status,
            [FromQuery] Severity? severity,
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            IMediator m = default!,
            CancellationToken ct = default) =>
        {
            var result = await m.Send(
                new ListIncidentsQuery(status, severity, search, page, pageSize), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(new { error = result.Error });
        })
        .WithName("ListIncidents")
        .WithSummary("Lista incidencias con filtros opcionales.")
        .Produces<PagedResult<IncidentListItem>>();

        // POST /api/incidents
        g.MapPost("/", async (
            CreateIncidentCommand cmd, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(cmd, ct);
            return result.IsSuccess
                ? Results.Created($"/api/incidents/{result.Value!.Id}", result.Value)
                : Results.BadRequest(new { error = result.Error });
        })
        .WithName("CreateIncident")
        .WithSummary("Crea una nueva incidencia.")
        .Produces<IncidentResponse>();

        // GET /api/incidents/{id}
        g.MapGet("/{id:guid}", async (
            Guid id, IMediator m, CancellationToken ct) =>
        {
            var result = await m.Send(new GetIncidentQuery(id), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound(new { error = result.Error });
        })
        .WithName("GetIncident")
        .WithSummary("Detalle de una incidencia.")
        .Produces<IncidentResponse>();

        // PATCH /api/incidents/{id}/status
        g.MapPatch("/{id:guid}/status", async (
            Guid id,
            [FromBody] UpdateIncidentStatusCommand cmd,
            IMediator m,
            CancellationToken ct) =>
        {
            var result = await m.Send(cmd with { IncidentId = id }, ct);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(new { error = result.Error });
        })
        .WithName("UpdateIncidentStatus")
        .WithSummary("Cambia el estado de una incidencia.");

        // PATCH /api/incidents/{id}/assign
        g.MapPatch("/{id:guid}/assign", async (
            Guid id,
            [FromBody] AssignIncidentCommand cmd,
            IMediator m,
            CancellationToken ct) =>
        {
            var result = await m.Send(cmd with { IncidentId = id }, ct);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(new { error = result.Error });
        })
        .WithName("AssignIncident")
        .WithSummary("Asigna una incidencia a un técnico. Solo TeamLead.");

        return app;
    }
}