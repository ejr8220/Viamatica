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

        builder.Property(a => a.AttentionTypeId)
            .HasColumnName("attentiontype_attentiontypeid")
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(a => a.StatusId)
            .HasColumnName("attentionstatus_statusid");

        // Relationships
        builder.HasOne(a => a.Turn)
            .WithMany(t => t.Attentions)
            .HasForeignKey(a => a.TurnId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Client)
            .WithMany(c => c.Attentions)
            .HasForeignKey(a => a.ClientId)
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
