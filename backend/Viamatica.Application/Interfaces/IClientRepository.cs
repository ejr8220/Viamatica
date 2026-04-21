using Viamatica.Application.DTOs.Clients;
using Viamatica.Domain.Entities;

namespace Viamatica.Application.Interfaces;

public interface IClientRepository
{
    Task<IReadOnlyCollection<ClientResponseDto>> GetAllAsync(string? identification, CancellationToken cancellationToken = default);
    Task<ClientResponseDto?> GetByIdAsync(int clientId, CancellationToken cancellationToken = default);
    Task<Client?> GetForUpdateAsync(int clientId, CancellationToken cancellationToken = default);
    Task<bool> IdentificationExistsAsync(string identification, int? currentClientId, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, int? currentClientId, CancellationToken cancellationToken = default);
    void Add(Client client);
    void Remove(Client client);
}
