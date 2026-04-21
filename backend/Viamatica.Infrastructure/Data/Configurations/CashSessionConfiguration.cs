using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Viamatica.Domain.Entities;

namespace Viamatica.Infrastructure.Data.Configurations;

public class CashSessionConfiguration : IEntityTypeConfiguration<CashSession>
{
    public void Configure(EntityTypeBuilder<CashSession> builder)
    {
        builder.ToTable("cashsession");

        builder.HasKey(entity => entity.CashSessionId);

        builder.Property(entity => entity.CashSessionId)
            .HasColumnName("cashsessionid");

        builder.Property(entity => entity.CashId)
            .HasColumnName("cash_cashid");

        builder.Property(entity => entity.UserId)
            .HasColumnName("user_userid");

        builder.Property(entity => entity.StartedAt)
            .HasColumnName("startedat");

        builder.Property(entity => entity.EndedAt)
            .HasColumnName("endedat");

        builder.HasIndex(entity => entity.CashId)
            .IsUnique()
            .HasFilter("[endedat] IS NULL");

        builder.HasOne(entity => entity.Cash)
            .WithMany(cash => cash.Sessions)
            .HasForeignKey(entity => entity.CashId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(entity => entity.User)
            .WithMany(user => user.CashSessions)
            .HasForeignKey(entity => entity.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
