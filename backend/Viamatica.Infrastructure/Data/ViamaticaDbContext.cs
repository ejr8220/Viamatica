using Microsoft.EntityFrameworkCore;
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
    }
}
