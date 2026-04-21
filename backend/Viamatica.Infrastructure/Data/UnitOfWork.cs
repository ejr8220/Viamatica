using Viamatica.Application.Interfaces;

namespace Viamatica.Infrastructure.Data;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ViamaticaDbContext _dbContext;

    public UnitOfWork(ViamaticaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
