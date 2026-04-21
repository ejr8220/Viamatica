using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Viamatica.Domain.Entities;

namespace Viamatica.Infrastructure.Data.Configurations;

public class UserStatusConfiguration : IEntityTypeConfiguration<UserStatus>
{
    public void Configure(EntityTypeBuilder<UserStatus> builder)
    {
        builder.ToTable("userstatus");

        builder.HasKey(us => us.StatusId);

        builder.Property(us => us.StatusId)
            .HasColumnName("statusid")
            .HasMaxLength(3)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(us => us.Description)
            .HasColumnName("description")
            .HasMaxLength(50)
            .IsRequired();
    }
}
