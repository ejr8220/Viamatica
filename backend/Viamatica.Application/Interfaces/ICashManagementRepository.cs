using Viamatica.Application.DTOs.Cashes;
using Viamatica.Domain.Entities;

namespace Viamatica.Application.Interfaces;

public interface ICashManagementRepository
{
    Task<IReadOnlyCollection<CashResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CashResponseDto?> GetByIdAsync(int cashId, CancellationToken cancellationToken = default);
    Task<Cash?> GetCashAsync(int cashId, CancellationToken cancellationToken = default);
    Task<Cash?> GetCashWithAssignmentsAsync(int cashId, CancellationToken cancellationToken = default);
    Task<Cash?> GetCashWithDependenciesAsync(int cashId, CancellationToken cancellationToken = default);
    Task<User?> GetUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<UserCash?> GetAssignmentAsync(int cashId, int userId, CancellationToken cancellationToken = default);
    Task<bool> HasActiveSessionAsync(int cashId, int userId, CancellationToken cancellationToken = default);
    Task<bool> IsCashierAssignedAsync(int cashId, int userId, CancellationToken cancellationToken = default);
    Task<CashSession?> GetActiveSessionByCashAsync(int cashId, CancellationToken cancellationToken = default);
    Task<CashSession?> GetActiveSessionAsync(int cashId, int userId, CancellationToken cancellationToken = default);
    Task<CashSessionResponseDto?> GetSessionByIdAsync(int cashSessionId, CancellationToken cancellationToken = default);
    void AddCash(Cash cash);
    void RemoveCash(Cash cash);
    void AddAssignment(UserCash assignment);
    void RemoveAssignment(UserCash assignment);
    Task AddSessionAsync(CashSession session, CancellationToken cancellationToken = default);
}
