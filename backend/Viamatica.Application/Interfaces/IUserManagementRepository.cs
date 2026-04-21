using Viamatica.Application.DTOs.Users;
using Viamatica.Domain.Entities;

namespace Viamatica.Application.Interfaces;

public interface IUserManagementRepository
{
    Task<IReadOnlyCollection<UserResponseDto>> GetAllAsync(bool pendingOnly, CancellationToken cancellationToken = default);
    Task<UserResponseDto?> GetByIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<User?> GetForUpdateAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> UserNameExistsAsync(string userName, int? currentUserId, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, int? currentUserId, CancellationToken cancellationToken = default);
    Task<bool> IdentificationExistsAsync(string identification, int? currentUserId, CancellationToken cancellationToken = default);
    Task<bool> RoleExistsAsync(int roleId, CancellationToken cancellationToken = default);
    Task<bool> IsActiveApprovedUserAsync(int userId, CancellationToken cancellationToken = default);
    void Add(User user);
    void Remove(User user);
}
