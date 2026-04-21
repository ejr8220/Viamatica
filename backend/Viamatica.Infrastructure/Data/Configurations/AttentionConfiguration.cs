using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Viamatica.Domain.Entities;

namespace Viamatica.Infrastructure.Data.Configurations;

public class AttentionConfiguration : IEntityTypeConfiguration<Attention>
{
    public void Configure(EntityTypeBuilder<Attention> builder)
    {
        builder.ToTable("attention");

        builder.HasKey(a => a.AttentionId);

        builder.Property(a => a.AttentionId)
            .HasColumnName("attentionid");

        builder.Property(a => a.TurnId)
            .HasColumnName("turn_turnid");

        builder.Property(a => a.ClientId)
            .HasColumnName("client_clientid");

        builder.Property(a => a.ContractId)
            .HasColumnName("contract_contractid");

        builder.Property(a => a.CashierUserId)
            .HasColumnName("cashier_userid");

        builder.Property(a => a.AttentionTypeId)
            .HasColumnName("attentiontype_attentiontypeid")
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(a => a.StatusId)
            .HasColumnName("attentionstatus_statusid");

        builder.Property(a => a.Notes)
            .HasColumnName("notes")
            .HasMaxLength(300);

        builder.Property(a => a.CreatedAt)
            .HasColumnName("createdat");

        builder.Property(a => a.CompletedAt)
            .HasColumnName("completedat");

        // Relationships
        builder.HasOne(a => a.Turn)
            .WithMany(t => t.Attentions)
            .HasForeignKey(a => a.TurnId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Client)
            .WithMany(c => c.Attentions)
            .HasForeignKey(a => a.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Contract)
            .WithMany(c => c.Attentions)
            .HasForeignKey(a => a.ContractId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.CashierUser)
            .WithMany(u => u.CashierAttentions)
            .HasForeignKey(a => a.CashierUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.AttentionType)
            .WithMany(at => at.Attentions)
            .HasForeignKey(a => a.AttentionTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Status)
            .WithMany(asEntity => asEntity.Attentions)
            .HasForeignKey(a => a.StatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
