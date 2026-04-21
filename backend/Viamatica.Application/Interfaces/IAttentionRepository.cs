using Viamatica.Application.DTOs.Attentions;
using Viamatica.Domain.Entities;

namespace Viamatica.Application.Interfaces;

public interface IAttentionRepository
{
    Task<IReadOnlyCollection<AttentionResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AttentionResponseDto?> GetByIdAsync(int attentionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AttentionTypeResponseDto>> GetTypesAsync(CancellationToken cancellationToken = default);
    Task<Turn?> GetTurnAsync(int turnId, CancellationToken cancellationToken = default);
    Task<Attention?> GetForUpdateAsync(int attentionId, CancellationToken cancellationToken = default);
    Task<bool> ClientExistsAsync(int clientId, CancellationToken cancellationToken = default);
    Task<bool> AttentionTypeExistsAsync(string attentionTypeId, CancellationToken cancellationToken = default);
    Task<bool> ContractBelongsToClientAsync(int contractId, int clientId, CancellationToken cancellationToken = default);
    Task<bool> HasOpenSessionAsync(int cashId, int cashierUserId, CancellationToken cancellationToken = default);
    Task<bool> OpenAttentionExistsAsync(int turnId, CancellationToken cancellationToken = default);
    void Add(Attention attention);
}
