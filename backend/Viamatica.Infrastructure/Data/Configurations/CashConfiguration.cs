using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Viamatica.Domain.Entities;

namespace Viamatica.Infrastructure.Data.Configurations;

public class CashConfiguration : IEntityTypeConfiguration<Cash>
{
    public void Configure(EntityTypeBuilder<Cash> builder)
    {
        builder.ToTable("cash");

        builder.HasKey(c => c.CashId);

        builder.Property(c => c.CashId)
            .HasColumnName("cashid");

        builder.Property(c => c.CashDescription)
            .HasColumnName("cashdescription")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Active)
            .HasColumnName("active")
            .HasMaxLength(1)
            .IsRequired();
    }
}
