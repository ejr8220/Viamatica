using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Viamatica.Domain.Entities;

namespace Viamatica.Infrastructure.Data.Configurations;

public class NavigationMenuRoleConfiguration : IEntityTypeConfiguration<NavigationMenuRole>
{
    public void Configure(EntityTypeBuilder<NavigationMenuRole> builder)
    {
        builder.ToTable("navigationmenurol");

        builder.HasKey(entity => new { entity.NavigationMenuId, entity.RoleId });

        builder.Property(entity => entity.NavigationMenuId)
            .HasColumnName("navigationmenuid");

        builder.Property(entity => entity.RoleId)
            .HasColumnName("rolid");

        builder.HasOne(entity => entity.NavigationMenu)
            .WithMany(entity => entity.NavigationMenuRoles)
            .HasForeignKey(entity => entity.NavigationMenuId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(entity => entity.Role)
            .WithMany(entity => entity.NavigationMenuRoles)
            .HasForeignKey(entity => entity.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
