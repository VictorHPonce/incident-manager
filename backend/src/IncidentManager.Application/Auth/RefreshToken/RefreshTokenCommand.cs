using IncidentManager.Application.Auth.DTOs;
using IncidentManager.Application.Common.Interfaces;

namespace IncidentManager.Application.Auth.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : ICommand<AuthResponse>;