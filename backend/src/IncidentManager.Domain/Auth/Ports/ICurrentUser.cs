namespace IncidentManager.Domain.Auth.Ports;

/// <summary>
/// Puerto de entrada: expone los datos del usuario autenticado actual.
/// Los Handlers lo inyectan para saber quién ejecuta la operación
/// sin depender de HttpContext directamente.
/// </summary>
public interface ICurrentUser
{
    Guid Id { get; }
    string Email { get; }
    string Role { get; }
    Guid TeamId { get; }
    bool IsAdmin => Role == "Admin";
    bool IsTeamLead => Role == "TeamLead" || Role == "Admin";
}