using Viamatica.Application.DTOs.Clients;

namespace Viamatica.Application.Interfaces;

public interface IClientService
{
    Task<IReadOnlyCollection<ClientResponseDto>> GetAllAsync(string? identification, CancellationToken cancellationToken = default);
    Task<ClientResponseDto> GetByIdAsync(int clientId, CancellationToken cancellationToken = default);
    Task<ClientResponseDto> CreateAsync(CreateClientRequestDto request, CancellationToken cancellationToken = default);
    Task<ClientResponseDto> UpdateAsync(int clientId, UpdateClientRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int clientId, CancellationToken cancellationToken = default);
}
