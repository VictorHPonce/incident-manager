using Microsoft.EntityFrameworkCore;
using IncidentManager.Domain.Incidents.Entities;
using IncidentManager.Domain.Teams.Entities;

namespace IncidentManager.Infrastructure.Persistence;

public sealed class IncidentManagerDbContext(DbContextOptions<IncidentManagerDbContext> options)
    : DbContext(options)
{
    public DbSet<Incident> Incidents => Set<Incident>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Convención global PascalCase → snake_case
        // Hace que Id → id, TeamId → team_id, CreatedAt → created_at, etc.
        // Sin esto Npgsql genera queries con mayúsculas que PostgreSQL rechaza.
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(ToSnakeCase(property.Name));
            }
        }

        // Las configuraciones individuales se aplican después — pueden sobreescribir
        // los nombres si algún caso especial lo requiere (como source_ref IS NOT NULL)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IncidentManagerDbContext).Assembly);
    }

    private static string ToSnakeCase(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;

        var result = new System.Text.StringBuilder();
        for (var i = 0; i < name.Length; i++)
        {
            if (i > 0 && char.IsUpper(name[i]) && !char.IsUpper(name[i - 1]))
                result.Append('_');
            result.Append(char.ToLower(name[i]));
        }
        return result.ToString();
    }
}