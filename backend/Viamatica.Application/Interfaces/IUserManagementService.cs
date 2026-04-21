using Viamatica.Application.DTOs.Users;

namespace Viamatica.Application.Interfaces;

public interface IUserManagementService
{
    Task<IReadOnlyCollection<UserResponseDto>> GetAllAsync(bool pendingOnly, CancellationToken cancellationToken = default);
    Task<UserResponseDto> GetByIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ActiveUserReportDto>> GetActiveReportAsync(CancellationToken cancellationToken = default);
    Task<UserResponseDto> CreateAsync(CreateUserRequestDto request, int creatorUserId, string creatorRole, CancellationToken cancellationToken = default);
    Task<UserResponseDto> UpdateAsync(int userId, UpdateUserRequestDto request, string actorRole, CancellationToken cancellationToken = default);
    Task ApproveAsync(int userId, CancellationToken cancellationToken = default);
    Task DeleteAsync(int userId, string actorRole, CancellationToken cancellationToken = default);
}
