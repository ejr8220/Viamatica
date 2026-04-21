using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Viamatica.Domain.Entities;

namespace Viamatica.Infrastructure.Data.Configurations;

public class MethodPaymentConfiguration : IEntityTypeConfiguration<MethodPayment>
{
    public void Configure(EntityTypeBuilder<MethodPayment> builder)
    {
        builder.ToTable("methodpayment");

        builder.HasKey(mp => mp.MethodPaymentId);

        builder.Property(mp => mp.MethodPaymentId)
            .HasColumnName("methodpaymentid");

        builder.Property(mp => mp.Description)
            .HasColumnName("description")
            .HasMaxLength(50)
            .IsRequired();
    }
}
