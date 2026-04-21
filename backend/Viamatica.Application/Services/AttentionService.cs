using Viamatica.Application.Common;
using Viamatica.Application.DTOs.Attentions;
using Viamatica.Application.Interfaces;
using Viamatica.Domain.Entities;

namespace Viamatica.Application.Services;

public sealed class AttentionService : IAttentionService
{
    private readonly IAttentionRepository _attentionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AttentionService(IAttentionRepository attentionRepository, IUnitOfWork unitOfWork)
    {
        _attentionRepository = attentionRepository;
        _unitOfWork = unitOfWork;
    }

    public Task<IReadOnlyCollection<AttentionResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => _attentionRepository.GetAllAsync(cancellationToken);

    public async Task<AttentionResponseDto> GetByIdAsync(int attentionId, CancellationToken cancellationToken = default)
    {
        var attention = await _attentionRepository.GetByIdAsync(attentionId, cancellationToken);
        return attention ?? throw new NotFoundException($"No se encontró la atención {attentionId}.");
    }

    public Task<IReadOnlyCollection<AttentionTypeResponseDto>> GetTypesAsync(CancellationToken cancellationToken = default)
        => _attentionRepository.GetTypesAsync(cancellationToken);

    public async Task<AttentionResponseDto> StartAsync(StartAttentionRequestDto request, int cashierUserId, string actorRole, CancellationToken cancellationToken = default)
    {
        ValidateCashierRole(actorRole);

        var turn = await _attentionRepository.GetTurnAsync(request.TurnId, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el turno {request.TurnId}.");

        var clientExists = await _attentionRepository.ClientExistsAsync(request.ClientId, cancellationToken);
        if (!clientExists)
        {
            throw new NotFoundException($"No se encontró el cliente {request.ClientId}.");
        }

        if (!await _attentionRepository.AttentionTypeExistsAsync(request.AttentionTypeId, cancellationToken))
        {
            throw new NotFoundException($"No se encontró el tipo de atención {request.AttentionTypeId}.");
        }

        if (request.ContractId.HasValue)
        {
            var contractExists = await _attentionRepository.ContractBelongsToClientAsync(request.ContractId.Value, request.ClientId, cancellationToken);

            if (!contractExists)
            {
                throw new BusinessRuleException("El contrato indicado no pertenece al cliente.");
            }
        }

        var hasOpenSession = await _attentionRepository.HasOpenSessionAsync(turn.CashId, cashierUserId, cancellationToken);

        if (!hasOpenSession)
        {
            throw new ForbiddenOperationException("El cajero debe tener una sesión activa en la caja del turno.");
        }

        var openAttentionExists = await _attentionRepository.OpenAttentionExistsAsync(request.TurnId, cancellationToken);

        if (openAttentionExists)
        {
            throw new ConflictException("El turno ya tiene una atención abierta.");
        }

        var attention = new Attention(
            request.TurnId,
            request.ClientId,
            request.ContractId,
            cashierUserId,
            request.AttentionTypeId.Trim().ToUpperInvariant(),
            AttentionStatusIds.Open,
            request.Notes);

        _attentionRepository.Add(attention);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(attention.AttentionId, cancellationToken);
    }

    public async Task<AttentionResponseDto> CompleteAsync(int attentionId, CloseAttentionRequestDto request, int cashierUserId, string actorRole, CancellationToken cancellationToken = default)
    {
        var attention = await GetAttentionForUpdateAsync(attentionId, cashierUserId, actorRole, cancellationToken);
        attention.Complete(request.Notes);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(attention.AttentionId, cancellationToken);
    }

    public async Task<AttentionResponseDto> CancelAsync(int attentionId, CloseAttentionRequestDto request, int cashierUserId, string actorRole, CancellationToken cancellationToken = default)
    {
        var attention = await GetAttentionForUpdateAsync(attentionId, cashierUserId, actorRole, cancellationToken);
        attention.Cancel(request.Notes);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(attention.AttentionId, cancellationToken);
    }

    private async Task<Attention> GetAttentionForUpdateAsync(int attentionId, int cashierUserId, string actorRole, CancellationToken cancellationToken)
    {
        var attention = await _attentionRepository.GetForUpdateAsync(attentionId, cancellationToken)
            ?? throw new NotFoundException($"No se encontró la atención {attentionId}.");

        if (attention.StatusId != AttentionStatusIds.Open)
        {
            throw new BusinessRuleException("Solo se pueden cerrar atenciones abiertas.");
        }

        if (attention.CashierUserId != cashierUserId && actorRole != RoleNames.Administrator)
        {
            throw new ForbiddenOperationException("Solo el cajero que abrió la atención o un administrador puede cerrarla.");
        }

        return attention;
    }

    private static void ValidateCashierRole(string actorRole)
    {
        if (actorRole is not RoleNames.Cashier and not RoleNames.Administrator)
        {
            throw new ForbiddenOperationException("Solo un cajero puede iniciar atenciones.");
        }
    }
}
