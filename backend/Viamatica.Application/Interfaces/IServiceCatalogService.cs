using Viamatica.Application.DTOs.Services;

namespace Viamatica.Application.Interfaces;

public interface IServiceCatalogService
{
    Task<IReadOnlyCollection<ServiceResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ServiceResponseDto> GetByIdAsync(int serviceId, CancellationToken cancellationToken = default);
    Task<ServiceResponseDto> CreateAsync(CreateServiceRequestDto request, CancellationToken cancellationToken = default);
    Task<ServiceResponseDto> UpdateAsync(int serviceId, UpdateServiceRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int serviceId, CancellationToken cancellationToken = default);
}
