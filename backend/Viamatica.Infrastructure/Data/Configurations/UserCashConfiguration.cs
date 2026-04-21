using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Viamatica.Domain.Entities;

namespace Viamatica.Infrastructure.Data.Configurations;

public class UserCashConfiguration : IEntityTypeConfiguration<UserCash>
{
    public void Configure(EntityTypeBuilder<UserCash> builder)
    {
        builder.ToTable("usercash");

        // Composite Primary Key
        builder.HasKey(uc => new { uc.UserId, uc.CashId });

        builder.Property(uc => uc.UserId)
            .HasColumnName("user_userid");

        builder.Property(uc => uc.CashId)
            .HasColumnName("cash_cashid");

        // Relationships
        builder.HasOne(uc => uc.User)
            .WithMany(u => u.UserCashes)
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(uc => uc.Cash)
            .WithMany(c => c.UserCashes)
            .HasForeignKey(uc => uc.CashId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
