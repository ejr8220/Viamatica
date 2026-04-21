using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Viamatica.Domain.Entities;

namespace Viamatica.Infrastructure.Data.Configurations;

public class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("device");

        builder.HasKey(d => d.DeviceId);

        builder.Property(d => d.DeviceId)
            .HasColumnName("deviceid");

        builder.Property(d => d.DeviceName)
            .HasColumnName("devicename")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(d => d.ServiceId)
            .HasColumnName("service_serviceid");

        // Relationships
        builder.HasOne(d => d.Service)
            .WithMany(s => s.Devices)
            .HasForeignKey(d => d.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
