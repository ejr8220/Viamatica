using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Viamatica.Domain.Entities;

namespace Viamatica.Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");

        builder.HasKey(p => p.PaymentId);

        builder.Property(p => p.PaymentId)
            .HasColumnName("paymentid");

        builder.Property(p => p.PaymentDate)
            .HasColumnName("paymentdate");

        builder.Property(p => p.ClientId)
            .HasColumnName("client_clientid");

        // Relationships
        builder.HasOne(p => p.Client)
            .WithMany(c => c.Payments)
            .HasForeignKey(p => p.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
