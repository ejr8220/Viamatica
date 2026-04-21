using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Viamatica.Application.Common;
using Viamatica.Application.DTOs.Attentions;
using Viamatica.Application.Interfaces;
using Viamatica.Domain.Entities;
using Viamatica.Infrastructure.Data;

namespace Viamatica.Infrastructure.Repositories;

public sealed class AttentionRepository : IAttentionRepository
{
    private readonly ViamaticaDbContext _dbContext;

    public AttentionRepository(ViamaticaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<AttentionResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbContext.Attentions
            .AsNoTracking()
            .Include(attention => attention.Turn)
            .Include(attention => attention.Client)
            .Include(attention => attention.CashierUser)
            .Include(attention => attention.AttentionType)
            .Include(attention => attention.Status)
            .OrderByDescending(attention => attention.CreatedAt)
            .Select(MapAttention())
            .ToListAsync(cancellationToken);

    public Task<AttentionResponseDto?> GetByIdAsync(int attentionId, CancellationToken cancellationToken = default)
        => _dbContext.Attentions
            .AsNoTracking()
            .Include(entity => entity.Turn)
            .Include(entity => entity.Client)
            .Include(entity => entity.CashierUser)
            .Include(entity => entity.AttentionType)
            .Include(entity => entity.Status)
            .Where(entity => entity.AttentionId == attentionId)
            .Select(MapAttention())
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyCollection<AttentionTypeResponseDto>> GetTypesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.AttentionTypes
            .AsNoTracking()
            .OrderBy(type => type.AttentionTypeId)
            .Select(type => new AttentionTypeResponseDto
            {
                AttentionTypeId = type.AttentionTypeId,
                Description = type.Description
            })
            .ToListAsync(cancellationToken);

    public Task<Turn?> GetTurnAsync(int turnId, CancellationToken cancellationToken = default)
        => _dbContext.Turns.FirstOrDefaultAsync(entity => entity.TurnId == turnId, cancellationToken);

    public Task<Attention?> GetForUpdateAsync(int attentionId, CancellationToken cancellationToken = default)
        => _dbContext.Attentions.FirstOrDefaultAsync(entity => entity.AttentionId == attentionId, cancellationToken);

    public Task<bool> ClientExistsAsync(int clientId, CancellationToken cancellationToken = default)
        => _dbContext.Clients.AnyAsync(client => client.ClientId == clientId, cancellationToken);

    public Task<bool> AttentionTypeExistsAsync(string attentionTypeId, CancellationToken cancellationToken = default)
        => _dbContext.AttentionTypes.AnyAsync(type => type.AttentionTypeId == attentionTypeId, cancellationToken);

    public Task<bool> ContractBelongsToClientAsync(int contractId, int clientId, CancellationToken cancellationToken = default)
        => _dbContext.Contracts.AnyAsync(contract => contract.ContractId == contractId && contract.ClientId == clientId, cancellationToken);

    public Task<bool> HasOpenSessionAsync(int cashId, int cashierUserId, CancellationToken cancellationToken = default)
        => _dbContext.CashSessions.AnyAsync(
            session => session.CashId == cashId && session.UserId == cashierUserId && session.EndedAt == null,
            cancellationToken);

    public Task<bool> OpenAttentionExistsAsync(int turnId, CancellationToken cancellationToken = default)
        => _dbContext.Attentions.AnyAsync(
            attention => attention.TurnId == turnId && attention.StatusId == AttentionStatusIds.Open,
            cancellationToken);

    public void Add(Attention attention) => _dbContext.Attentions.Add(attention);

    private static Expression<Func<Attention, AttentionResponseDto>> MapAttention()
    {
        return attention => new AttentionResponseDto
        {
            AttentionId = attention.AttentionId,
            TurnId = attention.TurnId,
            TurnDescription = attention.Turn.Description,
            ClientId = attention.ClientId,
            ClientName = attention.Client.Name + " " + attention.Client.LastName,
            ContractId = attention.ContractId,
            CashierUserId = attention.CashierUserId,
            CashierUserName = attention.CashierUser.UserName,
            AttentionTypeId = attention.AttentionTypeId,
            AttentionTypeDescription = attention.AttentionType.Description,
            StatusId = attention.StatusId,
            StatusDescription = attention.Status.Description,
            Notes = attention.Notes,
            CreatedAt = attention.CreatedAt,
            CompletedAt = attention.CompletedAt
        };
    }
}
