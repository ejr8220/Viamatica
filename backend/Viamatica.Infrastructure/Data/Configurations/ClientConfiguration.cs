using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Viamatica.Domain.Entities;

namespace Viamatica.Infrastructure.Data.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("client");

        builder.HasKey(c => c.ClientId);

        builder.Property(c => c.ClientId)
            .HasColumnName("clientid");

        builder.Property(c => c.Name)
            .HasColumnName("name")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.LastName)
            .HasColumnName("lastname")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.Identification)
            .HasColumnName("identification")
            .HasMaxLength(13)
            .IsRequired();

        builder.Property(c => c.Email)
            .HasColumnName("email")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.PhoneNumber)
            .HasColumnName("phonenumber")
            .HasMaxLength(13)
            .IsRequired();

        builder.Property(c => c.Address)
            .HasColumnName("address")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.ReferenceAddress)
            .HasColumnName("referenceaddress")
            .HasMaxLength(100)
            .IsRequired();

        // Index
        builder.HasIndex(c => c.Identification).IsUnique();
        builder.HasIndex(c => c.Email).IsUnique();
    }
}
