using System.Security.Claims;
using IncidentManager.Domain.Auth.Ports;
using Microsoft.AspNetCore.Http;

namespace IncidentManager.Infrastructure.Auth;

internal sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor)
    : ICurrentUser
{
    private ClaimsPrincipal User =>
        httpContextAccessor.HttpContext?.User
        ?? throw new InvalidOperationException(
            "CurrentUserService usado fuera de un contexto HTTP.");

    public Guid Id => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                            ?? User.FindFirstValue("sub")
                            ?? throw new InvalidOperationException("Claim 'sub' no encontrado."));

    public string Email => User.FindFirstValue(ClaimTypes.Email)
                            ?? User.FindFirstValue("email")
                            ?? string.Empty;

    public string Role => User.FindFirstValue(ClaimTypes.Role)
                            ?? string.Empty;

    public Guid TeamId => Guid.TryParse(User.FindFirstValue("team_id"), out var id)
                            ? id
                            : Guid.Empty;
}