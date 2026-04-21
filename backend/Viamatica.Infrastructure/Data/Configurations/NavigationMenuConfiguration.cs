using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Viamatica.Domain.Entities;

namespace Viamatica.Infrastructure.Data.Configurations;

public class NavigationMenuConfiguration : IEntityTypeConfiguration<NavigationMenu>
{
    public void Configure(EntityTypeBuilder<NavigationMenu> builder)
    {
        builder.ToTable("navigationmenu");

        builder.HasKey(entity => entity.NavigationMenuId);

        builder.Property(entity => entity.NavigationMenuId)
            .HasColumnName("navigationmenuid");

        builder.Property(entity => entity.MenuKey)
            .HasColumnName("menukey")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(entity => entity.Label)
            .HasColumnName("label")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(entity => entity.Icon)
            .HasColumnName("icon")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(entity => entity.Route)
            .HasColumnName("route")
            .HasMaxLength(200);

        builder.Property(entity => entity.DisplayOrder)
            .HasColumnName("displayorder")
            .IsRequired();

        builder.Property(entity => entity.IsActive)
            .HasColumnName("isactive")
            .IsRequired();

        builder.Property(entity => entity.ParentNavigationMenuId)
            .HasColumnName("parentnavigationmenuid");

        builder.HasIndex(entity => entity.MenuKey)
            .IsUnique();

        builder.HasOne(entity => entity.ParentNavigationMenu)
            .WithMany(entity => entity.Children)
            .HasForeignKey(entity => entity.ParentNavigationMenuId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
