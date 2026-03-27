using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IncidentManager.Domain.Teams.Entities;

namespace IncidentManager.Infrastructure.Persistence.Configurations.Auth;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("users", "auth");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever().HasColumnName("id");
        b.Property(x => x.Email).HasMaxLength(255).IsRequired().HasColumnName("email");
        b.Property(x => x.DisplayName).HasMaxLength(100).IsRequired().HasColumnName("display_name");
        b.Property(x => x.PasswordHash).HasMaxLength(255).IsRequired().HasColumnName("password_hash");
        b.Property(x => x.Role).HasConversion<string>().HasMaxLength(20).IsRequired().HasColumnName("role");
        b.Property(x => x.TeamId).HasColumnName("team_id");
        b.Property(x => x.IsActive).HasColumnName("is_active");
        b.Property(x => x.CreatedAt).HasColumnName("created_at");
        b.HasIndex(x => x.Email).IsUnique();
        b.HasIndex(x => x.TeamId);
    }
}
