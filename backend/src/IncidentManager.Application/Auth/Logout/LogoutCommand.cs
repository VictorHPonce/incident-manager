using IncidentManager.Application.Common.Interfaces;

namespace IncidentManager.Application.Auth.Logout;

public record LogoutCommand(string RefreshToken) : ICommand;