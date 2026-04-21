using Microsoft.EntityFrameworkCore;
using Viamatica.Application.Interfaces;
using Viamatica.Domain.Entities;
using Viamatica.Infrastructure.Data;

namespace Viamatica.Infrastructure.Repositories;

public sealed class UserAuthenticationRepository : IUserAuthenticationRepository
{
    private readonly ViamaticaDbContext _dbContext;

    public UserAuthenticationRepository(ViamaticaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<User?> GetByUserNameOrEmailAsync(string userNameOrEmail, CancellationToken cancellationToken = default)
    {
        var normalizedValue = userNameOrEmail.Trim();

        return _dbContext.Users
            .Include(user => user.Role)
            .Include(user => user.Status)
            .FirstOrDefaultAsync(
                user => user.UserName == normalizedValue || user.Email == normalizedValue,
                cancellationToken);
    }
}
