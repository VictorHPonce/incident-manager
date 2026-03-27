using IncidentManager.Domain.Auth.Ports;
using BC = BCrypt.Net.BCrypt;

namespace IncidentManager.Infrastructure.Auth;

internal sealed class BcryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BC.HashPassword(password);
    public bool Verify(string password, string hash) => BC.Verify(password, hash);
}