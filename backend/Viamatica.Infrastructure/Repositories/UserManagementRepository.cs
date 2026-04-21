using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Viamatica.Application.Common;
using Viamatica.Application.DTOs.Users;
using Viamatica.Application.Interfaces;
using Viamatica.Domain.Entities;
using Viamatica.Infrastructure.Data;

namespace Viamatica.Infrastructure.Repositories;

public sealed class UserManagementRepository : IUserManagementRepository
{
    private readonly ViamaticaDbContext _dbContext;

    public UserManagementRepository(ViamaticaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<UserResponseDto>> GetAllAsync(bool pendingOnly, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Users
            .AsNoTracking()
            .Include(user => user.Role)
            .Include(user => user.Status)
            .AsQueryable();

        if (pendingOnly)
        {
            query = query.Where(user => user.UserApproval == 0);
        }

        return await query
            .OrderBy(user => user.UserId)
            .Select(Map)
            .ToListAsync(cancellationToken);
    }

    public Task<UserResponseDto?> GetByIdAsync(int userId, CancellationToken cancellationToken = default)
        => _dbContext.Users
            .AsNoTracking()
            .Include(entity => entity.Role)
            .Include(entity => entity.Status)
            .Where(entity => entity.UserId == userId)
            .Select(Map)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<User?> GetForUpdateAsync(int userId, CancellationToken cancellationToken = default)
        => _dbContext.Users.FirstOrDefaultAsync(entity => entity.UserId == userId, cancellationToken);

    public Task<bool> UserNameExistsAsync(string userName, int? currentUserId, CancellationToken cancellationToken = default)
        => _dbContext.Users.AnyAsync(
            user => user.UserName == userName && (!currentUserId.HasValue || user.UserId != currentUserId.Value),
            cancellationToken);

    public Task<bool> EmailExistsAsync(string email, int? currentUserId, CancellationToken cancellationToken = default)
        => _dbContext.Users.AnyAsync(
            user => user.Email == email && (!currentUserId.HasValue || user.UserId != currentUserId.Value),
            cancellationToken);

    public Task<bool> RoleExistsAsync(int roleId, CancellationToken cancellationToken = default)
        => _dbContext.Roles.AnyAsync(role => role.RoleId == roleId, cancellationToken);

    public Task<bool> IsActiveApprovedUserAsync(int userId, CancellationToken cancellationToken = default)
        => _dbContext.Users.AnyAsync(
            user => user.UserId == userId &&
                    user.UserApproval == 1 &&
                    user.StatusId == UserStatusIds.Active,
            cancellationToken);

    public void Add(User user) => _dbContext.Users.Add(user);

    public void Remove(User user) => _dbContext.Users.Remove(user);

    private static readonly Expression<Func<User, UserResponseDto>> Map = user => new UserResponseDto
    {
        UserId = user.UserId,
        UserName = user.UserName,
        Email = user.Email,
        RoleId = user.RoleId,
        RoleName = user.Role.RoleName,
        StatusId = user.StatusId,
        StatusDescription = user.Status.Description,
        Approved = user.UserApproval == 1,
        DateApproval = user.DateApproval
    };
}
