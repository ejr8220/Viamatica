using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Viamatica.Application.Common;
using Viamatica.Application.DTOs.Cashes;
using Viamatica.Application.Interfaces;
using Viamatica.Domain.Entities;
using Viamatica.Infrastructure.Data;

namespace Viamatica.Infrastructure.Repositories;

public sealed class CashManagementRepository : ICashManagementRepository
{
    private readonly ViamaticaDbContext _dbContext;

    public CashManagementRepository(ViamaticaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<CashResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbContext.Cashes
            .AsNoTracking()
            .Include(cash => cash.UserCashes)
                .ThenInclude(userCash => userCash.User)
            .Include(cash => cash.Sessions.Where(session => session.EndedAt == null))
                .ThenInclude(session => session.User)
            .OrderBy(cash => cash.CashId)
            .Select(MapCash())
            .ToListAsync(cancellationToken);

    public Task<CashResponseDto?> GetByIdAsync(int cashId, CancellationToken cancellationToken = default)
        => _dbContext.Cashes
            .AsNoTracking()
            .Include(entity => entity.UserCashes)
                .ThenInclude(userCash => userCash.User)
            .Include(entity => entity.Sessions.Where(session => session.EndedAt == null))
                .ThenInclude(session => session.User)
            .Where(entity => entity.CashId == cashId)
            .Select(MapCash())
            .FirstOrDefaultAsync(cancellationToken);

    public Task<Cash?> GetCashAsync(int cashId, CancellationToken cancellationToken = default)
        => _dbContext.Cashes.FirstOrDefaultAsync(entity => entity.CashId == cashId, cancellationToken);

    public Task<Cash?> GetCashWithAssignmentsAsync(int cashId, CancellationToken cancellationToken = default)
        => _dbContext.Cashes
            .Include(entity => entity.UserCashes)
            .FirstOrDefaultAsync(entity => entity.CashId == cashId, cancellationToken);

    public Task<Cash?> GetCashWithDependenciesAsync(int cashId, CancellationToken cancellationToken = default)
        => _dbContext.Cashes
            .Include(entity => entity.UserCashes)
            .Include(entity => entity.Turns)
            .Include(entity => entity.Sessions)
            .FirstOrDefaultAsync(entity => entity.CashId == cashId, cancellationToken);

    public Task<User?> GetUserAsync(int userId, CancellationToken cancellationToken = default)
        => _dbContext.Users.FirstOrDefaultAsync(entity => entity.UserId == userId, cancellationToken);

    public Task<UserCash?> GetAssignmentAsync(int cashId, int userId, CancellationToken cancellationToken = default)
        => _dbContext.UserCashes.FirstOrDefaultAsync(entity => entity.CashId == cashId && entity.UserId == userId, cancellationToken);

    public Task<bool> HasActiveSessionAsync(int cashId, int userId, CancellationToken cancellationToken = default)
        => _dbContext.CashSessions.AnyAsync(
            session => session.CashId == cashId && session.UserId == userId && session.EndedAt == null,
            cancellationToken);

    public Task<bool> IsCashierAssignedAsync(int cashId, int userId, CancellationToken cancellationToken = default)
        => _dbContext.UserCashes.AnyAsync(entity => entity.CashId == cashId && entity.UserId == userId, cancellationToken);

    public Task<CashSession?> GetActiveSessionByCashAsync(int cashId, CancellationToken cancellationToken = default)
        => _dbContext.CashSessions
            .Include(session => session.User)
            .FirstOrDefaultAsync(session => session.CashId == cashId && session.EndedAt == null, cancellationToken);

    public Task<CashSession?> GetActiveSessionAsync(int cashId, int userId, CancellationToken cancellationToken = default)
        => _dbContext.CashSessions
            .Include(session => session.User)
            .FirstOrDefaultAsync(
                session => session.CashId == cashId && session.UserId == userId && session.EndedAt == null,
                cancellationToken);

    public Task<CashSessionResponseDto?> GetSessionByIdAsync(int cashSessionId, CancellationToken cancellationToken = default)
        => _dbContext.CashSessions
            .AsNoTracking()
            .Include(entity => entity.User)
            .Where(entity => entity.CashSessionId == cashSessionId)
            .Select(entity => new CashSessionResponseDto
            {
                CashSessionId = entity.CashSessionId,
                CashId = entity.CashId,
                UserId = entity.UserId,
                UserName = entity.User.UserName,
                StartedAt = entity.StartedAt,
                EndedAt = entity.EndedAt,
                IsActive = entity.EndedAt == null
            })
            .FirstOrDefaultAsync(cancellationToken);

    public void AddCash(Cash cash) => _dbContext.Cashes.Add(cash);

    public void RemoveCash(Cash cash) => _dbContext.Cashes.Remove(cash);

    public void AddAssignment(UserCash assignment) => _dbContext.UserCashes.Add(assignment);

    public void RemoveAssignment(UserCash assignment) => _dbContext.UserCashes.Remove(assignment);

    public async Task AddSessionAsync(CashSession session, CancellationToken cancellationToken = default)
    {
        _dbContext.CashSessions.Add(session);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            throw new ConflictException("No fue posible abrir la caja porque ya existe una sesión activa.");
        }
    }

    private static Expression<Func<Cash, CashResponseDto>> MapCash()
    {
        return cash => new CashResponseDto
        {
            CashId = cash.CashId,
            CashDescription = cash.CashDescription,
            Active = cash.Active,
            AssignedCashiers = cash.UserCashes
                .OrderBy(entity => entity.UserId)
                .Select(entity => new CashAssignmentResponseDto
                {
                    UserId = entity.UserId,
                    UserName = entity.User.UserName
                })
                .ToList(),
            ActiveSession = cash.Sessions
                .Where(entity => entity.EndedAt == null)
                .Select(entity => new CashSessionResponseDto
                {
                    CashSessionId = entity.CashSessionId,
                    CashId = entity.CashId,
                    UserId = entity.UserId,
                    UserName = entity.User.UserName,
                    StartedAt = entity.StartedAt,
                    EndedAt = entity.EndedAt,
                    IsActive = entity.EndedAt == null
                })
                .FirstOrDefault()
        };
    }
}
