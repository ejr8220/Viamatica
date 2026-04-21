using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Viamatica.Domain.Common;
using Viamatica.Domain.Entities;

namespace Viamatica.Infrastructure.Data;

public class ViamaticaDbContext : DbContext
{
    public ViamaticaDbContext(DbContextOptions<ViamaticaDbContext> options) 
        : base(options)
    {
    }

    public DbSet<UserStatus> UserStatuses { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Cash> Cashes { get; set; }
    public DbSet<UserCash> UserCashes { get; set; }
    public DbSet<CashSession> CashSessions { get; set; }
    public DbSet<Turn> Turns { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Device> Devices { get; set; }
    public DbSet<MethodPayment> MethodPayments { get; set; }
    public DbSet<StatusContract> StatusContracts { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<AttentionType> AttentionTypes { get; set; }
    public DbSet<AttentionStatus> AttentionStatuses { get; set; }
    public DbSet<Attention> Attentions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ViamaticaDbContext).Assembly);
        ApplySoftDeleteQueryFilters(modelBuilder);
    }

    public override int SaveChanges()
    {
        ApplySoftDeleteRules();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ApplySoftDeleteRules();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplySoftDeleteRules();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ApplySoftDeleteRules();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ApplySoftDeleteRules()
    {
        var softDeleteEntries = ChangeTracker.Entries<ISoftDeletable>()
            .Where(entry => entry.State == EntityState.Deleted);

        foreach (var entry in softDeleteEntries)
        {
            entry.State = EntityState.Modified;
            entry.Entity.SoftDelete();
        }
    }

    private static void ApplySoftDeleteQueryFilters(ModelBuilder modelBuilder)
    {
        var softDeleteEntities = modelBuilder.Model
            .GetEntityTypes()
            .Where(entityType => typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType));

        foreach (var entityType in softDeleteEntities)
        {
            var method = typeof(ViamaticaDbContext)
                .GetMethod(nameof(SetSoftDeleteFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                .MakeGenericMethod(entityType.ClrType);

            method.Invoke(null, [modelBuilder]);
        }
    }

    private static void SetSoftDeleteFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class, ISoftDeletable
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(entity => !entity.IsDeleted);
    }
}
