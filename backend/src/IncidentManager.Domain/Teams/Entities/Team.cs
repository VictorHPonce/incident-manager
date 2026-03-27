namespace IncidentManager.Domain.Teams.Entities;

public sealed class Team
{
    public Guid           Id        { get; private set; }
    public string         Name      { get; private set; } = string.Empty;
    public string         Slug      { get; private set; } = string.Empty;
    public bool           IsActive  { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private Team() { } // EF Core

    public static Team Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre es obligatorio.", nameof(name));

        return new Team
        {
            Id        = Guid.NewGuid(),
            Name      = name.Trim(),
            Slug      = name.Trim().ToLowerInvariant().Replace(' ', '-'),
            IsActive  = true,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public void Deactivate() => IsActive = false;
}
