using Viamatica.Application.Common;
using Viamatica.Application.DTOs.Turns;
using Viamatica.Application.Interfaces;
using Viamatica.Domain.Entities;

namespace Viamatica.Application.Services;

public sealed class TurnService : ITurnService
{
    private readonly ITurnRepository _turnRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TurnService(ITurnRepository turnRepository, IUnitOfWork unitOfWork)
    {
        _turnRepository = turnRepository;
        _unitOfWork = unitOfWork;
    }

    public Task<IReadOnlyCollection<TurnResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => _turnRepository.GetAllAsync(cancellationToken);

    public async Task<TurnResponseDto> GetByIdAsync(int turnId, CancellationToken cancellationToken = default)
    {
        var turn = await _turnRepository.GetByIdAsync(turnId, cancellationToken);
        return turn ?? throw new NotFoundException($"No se encontró el turno {turnId}.");
    }

    public async Task<TurnResponseDto> CreateAsync(CreateTurnRequestDto request, int gestorUserId, string actorRole, CancellationToken cancellationToken = default)
    {
        ValidateStaffRole(actorRole);
        await EnsureCashAndGestorAsync(request.CashId, gestorUserId, cancellationToken);

        var turn = new Turn(request.Description.Trim(), request.Date, request.CashId, gestorUserId);
        _turnRepository.Add(turn);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(turn.TurnId, cancellationToken);
    }

    public async Task<TurnResponseDto> UpdateAsync(int turnId, UpdateTurnRequestDto request, int gestorUserId, string actorRole, CancellationToken cancellationToken = default)
    {
        ValidateStaffRole(actorRole);

        var turn = await _turnRepository.GetForUpdateAsync(turnId, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el turno {turnId}.");

        await EnsureCashAndGestorAsync(request.CashId, gestorUserId, cancellationToken);
        turn.Update(request.Description.Trim(), request.Date, request.CashId, gestorUserId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(turn.TurnId, cancellationToken);
    }

    public async Task DeleteAsync(int turnId, CancellationToken cancellationToken = default)
    {
        var turn = await _turnRepository.GetForUpdateAsync(turnId, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el turno {turnId}.");

        _turnRepository.Remove(turn);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureCashAndGestorAsync(int cashId, int gestorUserId, CancellationToken cancellationToken)
    {
        var cashExists = await _turnRepository.CashIsActiveAsync(cashId, cancellationToken);
        if (!cashExists)
        {
            throw new BusinessRuleException("La caja indicada no existe o está inactiva.");
        }

        var gestorExists = await _turnRepository.GestorIsActiveAsync(gestorUserId, cancellationToken);

        if (!gestorExists)
        {
            throw new ForbiddenOperationException("Solo un administrador o gestor activo puede administrar turnos.");
        }
    }

    private static void ValidateStaffRole(string actorRole)
    {
        if (actorRole is not RoleNames.Administrator and not RoleNames.Gestor)
        {
            throw new ForbiddenOperationException("No tiene permisos para administrar turnos.");
        }
    }
}
