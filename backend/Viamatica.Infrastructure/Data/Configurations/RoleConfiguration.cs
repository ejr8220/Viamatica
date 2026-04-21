using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Viamatica.Domain.Entities;

namespace Viamatica.Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("rol");

        builder.HasKey(r => r.RoleId);

        builder.Property(r => r.RoleId)
            .HasColumnName("rolid");

        builder.Property(r => r.RoleName)
            .HasColumnName("rolname")
            .HasMaxLength(50)
            .IsRequired();
    }
}
