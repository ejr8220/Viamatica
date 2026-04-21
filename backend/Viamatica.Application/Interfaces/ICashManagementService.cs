using Viamatica.Application.DTOs.Cashes;

namespace Viamatica.Application.Interfaces;

public interface ICashManagementService
{
    Task<IReadOnlyCollection<CashResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CashResponseDto> GetByIdAsync(int cashId, CancellationToken cancellationToken = default);
    Task<CashResponseDto> CreateAsync(CreateCashRequestDto request, CancellationToken cancellationToken = default);
    Task<CashResponseDto> UpdateAsync(int cashId, UpdateCashRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int cashId, CancellationToken cancellationToken = default);
    Task AssignCashierAsync(int cashId, int userId, CancellationToken cancellationToken = default);
    Task UnassignCashierAsync(int cashId, int userId, CancellationToken cancellationToken = default);
    Task<CashSessionResponseDto> OpenSessionAsync(int cashId, int userId, CancellationToken cancellationToken = default);
    Task<CashSessionResponseDto> CloseSessionAsync(int cashId, int userId, CancellationToken cancellationToken = default);
}
