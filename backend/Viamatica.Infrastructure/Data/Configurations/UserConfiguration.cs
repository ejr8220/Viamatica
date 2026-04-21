using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Viamatica.Domain.Entities;

namespace Viamatica.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("usertable");

        builder.HasKey(u => u.UserId);

        builder.Property(u => u.UserId)
            .HasColumnName("userid");

        builder.Property(u => u.UserName)
            .HasColumnName("username")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(u => u.Identification)
            .HasColumnName("identification")
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasColumnName("email")
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(u => u.EmailHash)
            .HasColumnName("emailhash")
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(u => u.IdentificationHash)
            .HasColumnName("identificationhash")
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(u => u.Password)
            .HasColumnName("password")
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(u => u.RoleId)
            .HasColumnName("rol_rolid");

        builder.Property(u => u.StatusId)
            .HasColumnName("userstatus_statusid")
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(u => u.UserApproval)
            .HasColumnName("userapproval");

        builder.Property(u => u.DateApproval)
            .HasColumnName("dateapproval");

        builder.Property(u => u.IsDeleted)
            .HasColumnName("isdeleted")
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(u => u.DeletedAt)
            .HasColumnName("deletedat");

        // Relationships
        builder.HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(u => u.Status)
            .WithMany(us => us.Users)
            .HasForeignKey(u => u.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        // Index
        builder.HasIndex(u => u.UserName)
            .IsUnique()
            .HasFilter("[isdeleted] = 0");

        builder.HasIndex(u => u.EmailHash)
            .IsUnique()
            .HasFilter("[isdeleted] = 0");

        builder.HasIndex(u => u.IdentificationHash)
            .IsUnique()
            .HasFilter("[isdeleted] = 0");
    }
}
