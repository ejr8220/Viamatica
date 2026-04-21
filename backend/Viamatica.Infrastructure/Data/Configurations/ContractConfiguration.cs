using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Viamatica.Domain.Entities;

namespace Viamatica.Infrastructure.Data.Configurations;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.ToTable("contract");

        builder.HasKey(c => c.ContractId);

        builder.Property(c => c.ContractId)
            .HasColumnName("contractid");

        builder.Property(c => c.StartDate)
            .HasColumnName("startdate");

        builder.Property(c => c.EndDate)
            .HasColumnName("enddate");

        builder.Property(c => c.ServiceId)
            .HasColumnName("service_serviceid");

        builder.Property(c => c.StatusId)
            .HasColumnName("statuscontract_statusid")
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(c => c.ClientId)
            .HasColumnName("client_clientid");

        builder.Property(c => c.MethodPaymentId)
            .HasColumnName("methodpayment_methodpaymentid");

        // Relationships
        builder.HasOne(c => c.Service)
            .WithMany(s => s.Contracts)
            .HasForeignKey(c => c.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Status)
            .WithMany(sc => sc.Contracts)
            .HasForeignKey(c => c.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Client)
            .WithMany(cl => cl.Contracts)
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.MethodPayment)
            .WithMany(mp => mp.Contracts)
            .HasForeignKey(c => c.MethodPaymentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
