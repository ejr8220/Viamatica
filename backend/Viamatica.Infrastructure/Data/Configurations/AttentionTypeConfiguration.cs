using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Viamatica.Domain.Entities;

namespace Viamatica.Infrastructure.Data.Configurations;

public class AttentionTypeConfiguration : IEntityTypeConfiguration<AttentionType>
{
    public void Configure(EntityTypeBuilder<AttentionType> builder)
    {
        builder.ToTable("attentiontype");

        builder.HasKey(at => at.AttentionTypeId);

        builder.Property(at => at.AttentionTypeId)
            .HasColumnName("attentiontypeid")
            .HasMaxLength(3)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(at => at.Description)
            .HasColumnName("description")
            .HasMaxLength(50)
            .IsRequired();
    }
}
