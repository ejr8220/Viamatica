using Viamatica.Application.DTOs.Turns;

namespace Viamatica.Application.Interfaces;

public interface ITurnService
{
    Task<IReadOnlyCollection<TurnResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TurnResponseDto> GetByIdAsync(int turnId, CancellationToken cancellationToken = default);
    Task<TurnResponseDto> CreateAsync(CreateTurnRequestDto request, int gestorUserId, string actorRole, CancellationToken cancellationToken = default);
    Task<TurnResponseDto> UpdateAsync(int turnId, UpdateTurnRequestDto request, int gestorUserId, string actorRole, CancellationToken cancellationToken = default);
    Task DeleteAsync(int turnId, CancellationToken cancellationToken = default);
}
