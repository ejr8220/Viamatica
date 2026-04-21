using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Viamatica.Domain.Entities;

namespace Viamatica.Infrastructure.Data.Configurations;

public class AttentionStatusConfiguration : IEntityTypeConfiguration<AttentionStatus>
{
    public void Configure(EntityTypeBuilder<AttentionStatus> builder)
    {
        builder.ToTable("attentionstatus");

        builder.HasKey(asEntity => asEntity.StatusId);

        builder.Property(asEntity => asEntity.StatusId)
            .HasColumnName("statusid");

        builder.Property(asEntity => asEntity.Description)
            .HasColumnName("description")
            .HasMaxLength(50)
            .IsRequired();
    }
}
