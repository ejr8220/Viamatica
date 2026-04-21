using Microsoft.EntityFrameworkCore;
using Viamatica.Application.Interfaces;
using Viamatica.Domain.Entities;
using Viamatica.Infrastructure.Data;
using Viamatica.Infrastructure.Security;

namespace Viamatica.Infrastructure.Repositories;

public sealed class UserAuthenticationRepository : IUserAuthenticationRepository
{
    private readonly ViamaticaDbContext _dbContext;
    private readonly DatabaseFieldProtector _databaseFieldProtector;

    public UserAuthenticationRepository(ViamaticaDbContext dbContext, DatabaseFieldProtector databaseFieldProtector)
    {
        _dbContext = dbContext;
        _databaseFieldProtector = databaseFieldProtector;
    }

    public Task<User?> GetByUserNameOrEmailAsync(string userNameOrEmail, CancellationToken cancellationToken = default)
    {
        var normalizedValue = userNameOrEmail.Trim();

        return _dbContext.Users
            .Include(user => user.Role)
            .Include(user => user.Status)
            .FirstOrDefaultAsync(
                user => user.UserName == normalizedValue || user.EmailHash == _databaseFieldProtector.ComputeHash(normalizedValue),
                cancellationToken);
    }
}
