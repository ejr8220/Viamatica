using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Viamatica.Domain.Entities;

namespace Viamatica.Infrastructure.Data.Configurations;

public class TurnConfiguration : IEntityTypeConfiguration<Turn>
{
    public void Configure(EntityTypeBuilder<Turn> builder)
    {
        builder.ToTable("turn");

        builder.HasKey(t => t.TurnId);

        builder.Property(t => t.TurnId)
            .HasColumnName("turnid");

        builder.Property(t => t.Description)
            .HasColumnName("description")
            .HasMaxLength(6)
            .IsRequired();

        builder.Property(t => t.AttentionTypeId)
            .HasColumnName("attentiontype_attentiontypeid")
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(t => t.Date)
            .HasColumnName("date");

        builder.Property(t => t.CashId)
            .HasColumnName("cash_cashid");

        builder.Property(t => t.UserGestorId)
            .HasColumnName("usergestorid");

        // Relationships
        builder.HasOne(t => t.Cash)
            .WithMany(c => c.Turns)
            .HasForeignKey(t => t.CashId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.UserGestor)
            .WithMany(u => u.Turns)
            .HasForeignKey(t => t.UserGestorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.AttentionType)
            .WithMany()
            .HasForeignKey(t => t.AttentionTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
