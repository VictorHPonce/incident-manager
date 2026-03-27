using IncidentManager.Application.Auth.DTOs;
using IncidentManager.Application.Common.Interfaces;

namespace IncidentManager.Application.Auth.Login;

public record LoginCommand(string Email, string Password) : ICommand<AuthResponse>;
