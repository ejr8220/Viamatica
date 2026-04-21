using Viamatica.Application.Common;
using Viamatica.Application.DTOs.Cashes;
using Viamatica.Application.Interfaces;
using Viamatica.Domain.Entities;

namespace Viamatica.Application.Services;

public sealed class CashManagementService : ICashManagementService
{
    private readonly ICashManagementRepository _cashManagementRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CashManagementService(ICashManagementRepository cashManagementRepository, IUnitOfWork unitOfWork)
    {
        _cashManagementRepository = cashManagementRepository;
        _unitOfWork = unitOfWork;
    }

    public Task<IReadOnlyCollection<CashResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => _cashManagementRepository.GetAllAsync(cancellationToken);

    public async Task<CashResponseDto> GetByIdAsync(int cashId, CancellationToken cancellationToken = default)
    {
        var cash = await _cashManagementRepository.GetByIdAsync(cashId, cancellationToken);
        return cash ?? throw new NotFoundException($"No se encontró la caja {cashId}.");
    }

    public async Task<CashResponseDto> CreateAsync(CreateCashRequestDto request, CancellationToken cancellationToken = default)
    {
        var cash = new Cash(request.CashDescription.Trim());
        _cashManagementRepository.AddCash(cash);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(cash.CashId, cancellationToken);
    }

    public async Task<CashResponseDto> UpdateAsync(int cashId, UpdateCashRequestDto request, CancellationToken cancellationToken = default)
    {
        var cash = await _cashManagementRepository.GetCashAsync(cashId, cancellationToken)
            ?? throw new NotFoundException($"No se encontró la caja {cashId}.");

        cash.Update(request.CashDescription.Trim(), request.Active.Trim().ToUpperInvariant());
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(cash.CashId, cancellationToken);
    }

    public async Task DeleteAsync(int cashId, CancellationToken cancellationToken = default)
    {
        var cash = await _cashManagementRepository.GetCashWithDependenciesAsync(cashId, cancellationToken)
            ?? throw new NotFoundException($"No se encontró la caja {cashId}.");

        if (cash.UserCashes.Any() || cash.Turns.Any() || cash.Sessions.Any())
        {
            throw new BusinessRuleException("No se puede eliminar una caja con asignaciones, turnos o sesiones.");
        }

        _cashManagementRepository.RemoveCash(cash);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task AssignCashierAsync(int cashId, int userId, CancellationToken cancellationToken = default)
    {
        var cash = await _cashManagementRepository.GetCashWithAssignmentsAsync(cashId, cancellationToken)
            ?? throw new NotFoundException($"No se encontró la caja {cashId}.");

        var user = await _cashManagementRepository.GetUserAsync(userId, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el usuario {userId}.");

        if (user.RoleId != RoleIds.Cashier)
        {
            throw new BusinessRuleException("Solo los cajeros pueden asignarse a una caja.");
        }

        if (user.UserApproval != 1 || user.StatusId != UserStatusIds.Active)
        {
            throw new BusinessRuleException("El cajero debe estar aprobado y activo.");
        }

        if (cash.UserCashes.Any(entity => entity.UserId == userId))
        {
            throw new ConflictException("El cajero ya está asignado a la caja.");
        }

        if (cash.UserCashes.Count >= 2)
        {
            throw new BusinessRuleException("La caja ya tiene el máximo de 2 cajeros asignados.");
        }

        _cashManagementRepository.AddAssignment(new UserCash(userId, cashId));
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UnassignCashierAsync(int cashId, int userId, CancellationToken cancellationToken = default)
    {
        var assignment = await _cashManagementRepository.GetAssignmentAsync(cashId, userId, cancellationToken)
            ?? throw new NotFoundException("No existe la asignación solicitada.");

        var hasActiveSession = await _cashManagementRepository.HasActiveSessionAsync(cashId, userId, cancellationToken);

        if (hasActiveSession)
        {
            throw new BusinessRuleException("No puede desasignar un cajero con sesión activa.");
        }

        _cashManagementRepository.RemoveAssignment(assignment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<CashSessionResponseDto> OpenSessionAsync(int cashId, int userId, CancellationToken cancellationToken = default)
    {
        var cash = await _cashManagementRepository.GetCashAsync(cashId, cancellationToken)
            ?? throw new NotFoundException($"No se encontró la caja {cashId}.");

        if (cash.Active != "Y")
        {
            throw new BusinessRuleException("La caja se encuentra inactiva.");
        }

        var user = await _cashManagementRepository.GetUserAsync(userId, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el usuario {userId}.");

        if (user.RoleId != RoleIds.Cashier)
        {
            throw new ForbiddenOperationException("Solo un cajero puede abrir una sesión de caja.");
        }

        var isAssigned = await _cashManagementRepository.IsCashierAssignedAsync(cashId, userId, cancellationToken);

        if (!isAssigned)
        {
            throw new BusinessRuleException("El cajero no está asignado a la caja.");
        }

        var activeSession = await _cashManagementRepository.GetActiveSessionByCashAsync(cashId, cancellationToken);

        if (activeSession is not null)
        {
            throw new ConflictException($"La caja ya está siendo usada por {activeSession.User.UserName}.");
        }

        var sessionEntity = new CashSession(cashId, userId);
        await _cashManagementRepository.AddSessionAsync(sessionEntity, cancellationToken);

        return await GetSessionAsync(sessionEntity.CashSessionId, cancellationToken);
    }

    public async Task<CashSessionResponseDto> CloseSessionAsync(int cashId, int userId, CancellationToken cancellationToken = default)
    {
        var sessionEntity = await _cashManagementRepository.GetActiveSessionAsync(cashId, userId, cancellationToken)
            ?? throw new NotFoundException("No existe una sesión activa de la caja para el usuario.");

        sessionEntity.Close();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await GetSessionAsync(sessionEntity.CashSessionId, cancellationToken);
    }

    private async Task<CashSessionResponseDto> GetSessionAsync(int cashSessionId, CancellationToken cancellationToken)
    {
        var session = await _cashManagementRepository.GetSessionByIdAsync(cashSessionId, cancellationToken);
        return session ?? throw new NotFoundException("No se encontró la sesión de caja.");
    }
}
