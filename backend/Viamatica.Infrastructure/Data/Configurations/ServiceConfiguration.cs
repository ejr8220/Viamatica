using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Viamatica.Domain.Entities;

namespace Viamatica.Infrastructure.Data.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("service");

        builder.HasKey(s => s.ServiceId);

        builder.Property(s => s.ServiceId)
            .HasColumnName("serviceid");

        builder.Property(s => s.ServiceName)
            .HasColumnName("servicename")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.ServiceDescription)
            .HasColumnName("servicedescription")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(s => s.Price)
            .HasColumnName("price")
            .HasColumnType("decimal(18,2)")
            .IsRequired();
    }
}
