using Viamatica.Application.DTOs.Attentions;

namespace Viamatica.Application.Interfaces;

public interface IAttentionService
{
    Task<IReadOnlyCollection<AttentionResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AttentionResponseDto> GetByIdAsync(int attentionId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AttentionTypeResponseDto>> GetTypesAsync(CancellationToken cancellationToken = default);
    Task<AttentionResponseDto> StartAsync(StartAttentionRequestDto request, int cashierUserId, string actorRole, CancellationToken cancellationToken = default);
    Task<AttentionResponseDto> CompleteAsync(int attentionId, CloseAttentionRequestDto request, int cashierUserId, string actorRole, CancellationToken cancellationToken = default);
    Task<AttentionResponseDto> CancelAsync(int attentionId, CloseAttentionRequestDto request, int cashierUserId, string actorRole, CancellationToken cancellationToken = default);
}
