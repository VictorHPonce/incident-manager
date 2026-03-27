namespace IncidentManager.Domain.Teams.Entities;

public enum UserRole { Technician, TeamLead, Admin }

public sealed class User
{
    public Guid           Id           { get; private set; }
    public string         Email        { get; private set; } = string.Empty;
    public string         DisplayName  { get; private set; } = string.Empty;
    public string         PasswordHash { get; private set; } = string.Empty;
    public UserRole       Role         { get; private set; }
    public Guid           TeamId       { get; private set; }
    public bool           IsActive     { get; private set; }
    public DateTimeOffset CreatedAt    { get; private set; }

    private User() { } // EF Core

    public static User Create(
        string email, string displayName, string passwordHash, UserRole role, Guid teamId)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email es obligatorio.", nameof(email));

        return new User
        {
            Id           = Guid.NewGuid(),
            Email        = email.Trim().ToLowerInvariant(),
            DisplayName  = displayName.Trim(),
            PasswordHash = passwordHash,
            Role         = role,
            TeamId       = teamId,
            IsActive     = true,
            CreatedAt    = DateTimeOffset.UtcNow
        };
    }

    public void ChangePasswordHash(string newHash) => PasswordHash = newHash;
    public void Deactivate() => IsActive = false;
}
