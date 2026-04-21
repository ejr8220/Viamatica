using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Viamatica.Domain.Entities;

namespace Viamatica.Infrastructure.Data.Configurations;

public class StatusContractConfiguration : IEntityTypeConfiguration<StatusContract>
{
    public void Configure(EntityTypeBuilder<StatusContract> builder)
    {
        builder.ToTable("statuscontract");

        builder.HasKey(sc => sc.StatusId);

        builder.Property(sc => sc.StatusId)
            .HasColumnName("statusid")
            .HasMaxLength(3)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(sc => sc.Description)
            .HasColumnName("description")
            .HasMaxLength(50)
            .IsRequired();
    }
}
