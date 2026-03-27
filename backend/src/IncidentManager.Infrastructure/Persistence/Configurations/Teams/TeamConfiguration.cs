using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IncidentManager.Domain.Teams.Entities;

namespace IncidentManager.Infrastructure.Persistence.Configurations.Teams;

internal sealed class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> b)
    {
        b.ToTable("teams", "teams");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever().HasColumnName("id");
        b.Property(x => x.Name).HasMaxLength(100).IsRequired().HasColumnName("name");
        b.Property(x => x.Slug).HasMaxLength(100).IsRequired().HasColumnName("slug");
        b.Property(x => x.IsActive).HasColumnName("is_active");
        b.Property(x => x.CreatedAt).HasColumnName("created_at");
        b.HasIndex(x => x.Slug).IsUnique();
    }
}
