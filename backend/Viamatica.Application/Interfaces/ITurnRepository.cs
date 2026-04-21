using Viamatica.Application.DTOs.Turns;
using Viamatica.Domain.Entities;

namespace Viamatica.Application.Interfaces;

public interface ITurnRepository
{
    Task<IReadOnlyCollection<TurnResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TurnResponseDto?> GetByIdAsync(int turnId, CancellationToken cancellationToken = default);
    Task<Turn?> GetForUpdateAsync(int turnId, CancellationToken cancellationToken = default);
    Task<bool> CashIsActiveAsync(int cashId, CancellationToken cancellationToken = default);
    Task<bool> GestorIsActiveAsync(int gestorUserId, CancellationToken cancellationToken = default);
    Task<bool> AttentionTypeExistsAsync(string attentionTypeId, CancellationToken cancellationToken = default);
    Task<int> GetNextSequenceForPrefixAsync(string prefix, int? excludedTurnId = null, CancellationToken cancellationToken = default);
    void Add(Turn turn);
    void Remove(Turn turn);
}
