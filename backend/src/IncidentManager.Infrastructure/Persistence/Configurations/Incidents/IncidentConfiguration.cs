using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IncidentManager.Domain.Incidents.Entities;

namespace IncidentManager.Infrastructure.Persistence.Configurations.Incidents;

internal sealed class IncidentConfiguration : IEntityTypeConfiguration<Incident>
{
    public void Configure(EntityTypeBuilder<Incident> b)
    {
        b.ToTable("incidents", "incidents");
        b.HasKey(x => new { x.Id, x.CreatedAt });
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.Title).HasMaxLength(255).IsRequired();
        b.Property(x => x.Description).HasMaxLength(2000);

        b.Property(x => x.Severity).HasConversion<string>().HasMaxLength(20).IsRequired();
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        b.Property(x => x.SourceType).HasConversion<string>().HasMaxLength(20).IsRequired();
        b.Property(x => x.SourceRef).HasMaxLength(500);

        // HasColumnName explícito en tipos especiales PostgreSQL
        // La convención global no los procesa correctamente cuando hay HasConversion
        b.Property(x => x.Tags)
            .HasColumnName("tags")
            .HasColumnType("text[]");

        b.Property(x => x.SourceMetadata)
            .HasColumnName("source_metadata")
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(
                         v, (JsonSerializerOptions?)null)
                     ?? new Dictionary<string, object>())
            .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<Dictionary<string, object>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToDictionary(entry => entry.Key, entry => entry.Value)));

        b.HasIndex(x => new { x.TeamId, x.Status });
        b.HasIndex(x => x.CreatedAt);
        b.HasIndex(x => x.SourceRef).HasFilter("source_ref IS NOT NULL");
    }
}