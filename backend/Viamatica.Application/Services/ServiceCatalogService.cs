using Viamatica.Application.Common;
using Viamatica.Application.DTOs.Services;
using Viamatica.Application.Interfaces;
using Viamatica.Domain.Entities;

namespace Viamatica.Application.Services;

public sealed class ServiceCatalogService : IServiceCatalogService
{
    private readonly IServiceCatalogRepository _serviceCatalogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ServiceCatalogService(IServiceCatalogRepository serviceCatalogRepository, IUnitOfWork unitOfWork)
    {
        _serviceCatalogRepository = serviceCatalogRepository;
        _unitOfWork = unitOfWork;
    }

    public Task<IReadOnlyCollection<ServiceResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => _serviceCatalogRepository.GetAllAsync(cancellationToken);

    public async Task<ServiceResponseDto> GetByIdAsync(int serviceId, CancellationToken cancellationToken = default)
    {
        var service = await _serviceCatalogRepository.GetByIdAsync(serviceId, cancellationToken);
        return service ?? throw new NotFoundException($"No se encontró el servicio {serviceId}.");
    }

    public async Task<ServiceResponseDto> CreateAsync(CreateServiceRequestDto request, CancellationToken cancellationToken = default)
    {
        var service = new Service(request.ServiceName.Trim(), request.ServiceDescription.Trim(), request.Price);
        _serviceCatalogRepository.Add(service);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(service.ServiceId, cancellationToken);
    }

    public async Task<ServiceResponseDto> UpdateAsync(int serviceId, UpdateServiceRequestDto request, CancellationToken cancellationToken = default)
    {
        var service = await _serviceCatalogRepository.GetForUpdateAsync(serviceId, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el servicio {serviceId}.");

        service.Update(request.ServiceName.Trim(), request.ServiceDescription.Trim(), request.Price);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(service.ServiceId, cancellationToken);
    }

    public async Task DeleteAsync(int serviceId, CancellationToken cancellationToken = default)
    {
        var service = await _serviceCatalogRepository.GetForUpdateAsync(serviceId, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el servicio {serviceId}.");

        if (await _serviceCatalogRepository.HasContractsAsync(serviceId, cancellationToken))
        {
            throw new BusinessRuleException("No se puede eliminar un servicio con contratos asociados.");
        }

        _serviceCatalogRepository.Remove(service);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
