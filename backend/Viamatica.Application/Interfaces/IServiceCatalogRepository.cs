using Viamatica.Application.DTOs.Services;
using Viamatica.Domain.Entities;

namespace Viamatica.Application.Interfaces;

public interface IServiceCatalogRepository
{
    Task<IReadOnlyCollection<ServiceResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ServiceResponseDto?> GetByIdAsync(int serviceId, CancellationToken cancellationToken = default);
    Task<Service?> GetForUpdateAsync(int serviceId, CancellationToken cancellationToken = default);
    Task<bool> HasContractsAsync(int serviceId, CancellationToken cancellationToken = default);
    void Add(Service service);
    void Remove(Service service);
}
