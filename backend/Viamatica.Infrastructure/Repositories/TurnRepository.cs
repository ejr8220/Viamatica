using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Viamatica.Application.Common;
using Viamatica.Application.DTOs.Turns;
using Viamatica.Application.Interfaces;
using Viamatica.Domain.Entities;
using Viamatica.Infrastructure.Data;

namespace Viamatica.Infrastructure.Repositories;

public sealed class TurnRepository : ITurnRepository
{
    private readonly ViamaticaDbContext _dbContext;

    public TurnRepository(ViamaticaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<TurnResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbContext.Turns
            .AsNoTracking()
            .Include(turn => turn.Cash)
            .Include(turn => turn.UserGestor)
            .OrderByDescending(turn => turn.Date)
            .Select(Map)
            .ToListAsync(cancellationToken);

    public Task<TurnResponseDto?> GetByIdAsync(int turnId, CancellationToken cancellationToken = default)
        => _dbContext.Turns
            .AsNoTracking()
            .Include(entity => entity.Cash)
            .Include(entity => entity.UserGestor)
            .Where(entity => entity.TurnId == turnId)
            .Select(Map)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<Turn?> GetForUpdateAsync(int turnId, CancellationToken cancellationToken = default)
        => _dbContext.Turns.FirstOrDefaultAsync(entity => entity.TurnId == turnId, cancellationToken);

    public Task<bool> CashIsActiveAsync(int cashId, CancellationToken cancellationToken = default)
        => _dbContext.Cashes.AnyAsync(cash => cash.CashId == cashId && cash.Active == "Y", cancellationToken);

    public Task<bool> GestorIsActiveAsync(int gestorUserId, CancellationToken cancellationToken = default)
        => _dbContext.Users.AnyAsync(
            user => user.UserId == gestorUserId &&
                    (user.RoleId == RoleIds.Administrator || user.RoleId == RoleIds.Gestor) &&
                    user.UserApproval == 1 &&
                    user.StatusId == UserStatusIds.Active,
            cancellationToken);

    public void Add(Turn turn) => _dbContext.Turns.Add(turn);

    public void Remove(Turn turn) => _dbContext.Turns.Remove(turn);

    private static readonly Expression<Func<Turn, TurnResponseDto>> Map = turn => new TurnResponseDto
    {
        TurnId = turn.TurnId,
        Description = turn.Description,
        Date = turn.Date,
        CashId = turn.CashId,
        CashDescription = turn.Cash.CashDescription,
        UserGestorId = turn.UserGestorId,
        GestorUserName = turn.UserGestor.UserName
    };
}
