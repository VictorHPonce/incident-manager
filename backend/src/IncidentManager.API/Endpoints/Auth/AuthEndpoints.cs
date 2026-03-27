using IncidentManager.Application.Auth.DTOs;
using IncidentManager.Application.Auth.Login;
using IncidentManager.Application.Auth.Logout;
using IncidentManager.Application.Auth.RefreshToken;
using MediatR;

namespace IncidentManager.API.Endpoints.Auth;

public static class AuthEndpoints
{
    private const string CookieName = "im_refresh";

    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var g = app.MapGroup("/api/auth").WithTags("Auth");

        // POST /api/auth/login
        g.MapPost("/login", async (
            LoginCommand cmd,
            IMediator m,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            var result = await m.Send(cmd, ct);
            if (result.IsFailure)
                return Results.Unauthorized();

            var value = result.Value!;

            // Refresh token → cookie HttpOnly (nunca al body)
            ctx.Response.Cookies.Append(CookieName, result.Value!.RefreshToken,
                RefreshCookieOptions(ctx));

            // Access token → body JSON (el frontend lo guarda en memoria)
            return Results.Ok(new LoginResult(
                value.AccessToken,
                value.ExpiresAt,
                value.Email,
                value.Role,
                value.TeamId
            ));
        })
        .WithName("Login")
        .WithSummary("Autentica y devuelve JWT + cookie de refresco.")
        .Produces<LoginResult>()
        .AllowAnonymous();

        // POST /api/auth/refresh
        g.MapPost("/refresh", async (
            IMediator m,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            // Lee el refresh token de la cookie — el body no lleva nada
            if (!ctx.Request.Cookies.TryGetValue(CookieName, out var refreshToken))
                return Results.Unauthorized();

            var result = await m.Send(new RefreshTokenCommand(refreshToken), ct);
            if (result.IsFailure)
            {
                // Token inválido — limpiar la cookie también
                ctx.Response.Cookies.Delete(CookieName);
                return Results.Unauthorized();
            }

            // Rotation: el Handler generó un nuevo par — actualizar la cookie
            ctx.Response.Cookies.Append(CookieName, result.Value!.RefreshToken,
                RefreshCookieOptions(ctx));

            return Results.Ok(new LoginResult(
                result.Value!.AccessToken,
                result.Value.ExpiresAt,
                result.Value.Email,
                result.Value.Role,
                result.Value.TeamId
            ));
        })
        .WithName("RefreshToken")
        .WithSummary("Renueva el access token usando la cookie de refresco.")
        .Produces<LoginResult>()
        .AllowAnonymous();

        // POST /api/auth/logout
        g.MapPost("/logout", async (
            IMediator m,
            HttpContext ctx,
            CancellationToken ct) =>
        {
            if (ctx.Request.Cookies.TryGetValue(CookieName, out var refreshToken))
            {
                await m.Send(new LogoutCommand(refreshToken), ct);
                ctx.Response.Cookies.Delete(CookieName);
            }
            // Siempre 200 — si no había cookie, el usuario ya estaba deslogado
            return Results.Ok();
        })
        .WithName("Logout")
        .WithSummary("Revoca la sesión activa.")
        .RequireAuthorization();

        return app;
    }

    // Opciones de cookie centralizadas — un solo sitio para cambiarlas
    private static CookieOptions RefreshCookieOptions(HttpContext ctx) => new()
    {
        HttpOnly = true,
        // Secure solo en producción — en localhost HTTP no funciona con Secure=true
        Secure = !ctx.Request.Host.Host.Contains("localhost"),
        SameSite = SameSiteMode.Strict,
        Expires = DateTimeOffset.UtcNow.AddDays(7),
        Path = "/api/auth"  // la cookie solo se envía en rutas de auth
    };
}