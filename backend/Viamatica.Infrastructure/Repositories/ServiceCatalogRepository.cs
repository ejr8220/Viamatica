using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Viamatica.Application.DTOs.Services;
using Viamatica.Application.Interfaces;
using Viamatica.Domain.Entities;
using Viamatica.Infrastructure.Data;

namespace Viamatica.Infrastructure.Repositories;

public sealed class ServiceCatalogRepository : IServiceCatalogRepository
{
    private readonly ViamaticaDbContext _dbContext;

    public ServiceCatalogRepository(ViamaticaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<ServiceResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbContext.Services
            .AsNoTracking()
            .OrderBy(service => service.ServiceId)
            .Select(Map)
            .ToListAsync(cancellationToken);

    public Task<ServiceResponseDto?> GetByIdAsync(int serviceId, CancellationToken cancellationToken = default)
        => _dbContext.Services
            .AsNoTracking()
            .Where(entity => entity.ServiceId == serviceId)
            .Select(Map)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<Service?> GetForUpdateAsync(int serviceId, CancellationToken cancellationToken = default)
        => _dbContext.Services.FirstOrDefaultAsync(entity => entity.ServiceId == serviceId, cancellationToken);

    public Task<bool> HasContractsAsync(int serviceId, CancellationToken cancellationToken = default)
        => _dbContext.Contracts.AnyAsync(entity => entity.ServiceId == serviceId, cancellationToken);

    public void Add(Service service) => _dbContext.Services.Add(service);

    public void Remove(Service service) => _dbContext.Services.Remove(service);

    private static readonly Expression<Func<Service, ServiceResponseDto>> Map = service => new ServiceResponseDto
    {
        ServiceId = service.ServiceId,
        ServiceName = service.ServiceName,
        ServiceDescription = service.ServiceDescription,
        Price = service.Price
    };
}
