using Viamatica.Application.Common;
using Viamatica.Application.DTOs.Contracts;
using Viamatica.Application.Interfaces;
using Viamatica.Domain.Entities;

namespace Viamatica.Application.Services;

public sealed class ContractService : IContractService
{
    private readonly IContractRepository _contractRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ContractService(IContractRepository contractRepository, IUnitOfWork unitOfWork)
    {
        _contractRepository = contractRepository;
        _unitOfWork = unitOfWork;
    }

    public Task<IReadOnlyCollection<ContractResponseDto>> GetAllAsync(int? clientId, CancellationToken cancellationToken = default)
        => _contractRepository.GetAllAsync(clientId, cancellationToken);

    public async Task<ContractResponseDto> GetByIdAsync(int contractId, CancellationToken cancellationToken = default)
    {
        var contract = await _contractRepository.GetByIdAsync(contractId, cancellationToken);
        return contract ?? throw new NotFoundException($"No se encontró el contrato {contractId}.");
    }

    public Task<IReadOnlyCollection<PaymentMethodResponseDto>> GetPaymentMethodsAsync(CancellationToken cancellationToken = default)
        => _contractRepository.GetPaymentMethodsAsync(cancellationToken);

    public async Task<ContractResponseDto> CreateAsync(CreateContractRequestDto request, CancellationToken cancellationToken = default)
    {
        await EnsureRelationsAsync(request.ClientId, request.ServiceId, request.MethodPaymentId, cancellationToken);

        var contract = new Contract(
            request.StartDate,
            request.EndDate,
            request.ServiceId,
            ContractStatusIds.Active,
            request.ClientId,
            request.MethodPaymentId);

        _contractRepository.AddContract(contract);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(contract.ContractId, cancellationToken);
    }

    public async Task<ContractResponseDto> UpdateAsync(int contractId, UpdateContractRequestDto request, CancellationToken cancellationToken = default)
    {
        var contract = await _contractRepository.GetForUpdateAsync(contractId, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el contrato {contractId}.");

        contract.UpdateDates(request.StartDate, request.EndDate);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(contract.ContractId, cancellationToken);
    }

    public async Task<PaymentResponseDto> RegisterPaymentAsync(int contractId, CreatePaymentRequestDto request, CancellationToken cancellationToken = default)
    {
        var contract = await _contractRepository.GetForUpdateAsync(contractId, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el contrato {contractId}.");

        if (!contract.IsOperative())
        {
            throw new BusinessRuleException("Solo se pueden registrar pagos sobre contratos operativos.");
        }

        var payment = new Payment(request.PaymentDate, contract.ClientId, contract.ContractId, request.Amount, request.Description);
        _contractRepository.AddPayment(payment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await _contractRepository.GetPaymentByIdAsync(payment.PaymentId, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el pago {payment.PaymentId}.");
    }

    public async Task<ContractResponseDto> ChangeServiceAsync(int contractId, ChangeContractServiceRequestDto request, CancellationToken cancellationToken = default)
    {
        var currentContract = await _contractRepository.GetForUpdateAsync(contractId, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el contrato {contractId}.");

        if (!currentContract.IsOperative())
        {
            throw new BusinessRuleException("Solo los contratos operativos pueden cambiar de servicio.");
        }

        if (currentContract.ServiceId == request.NewServiceId)
        {
            throw new BusinessRuleException("El nuevo servicio debe ser distinto al actual.");
        }

        var newServiceExists = await _contractRepository.ServiceExistsAsync(request.NewServiceId, cancellationToken);
        if (!newServiceExists)
        {
            throw new NotFoundException($"No se encontró el servicio {request.NewServiceId}.");
        }

        currentContract.MarkAsReplaced();

        var replacementContract = new Contract(
            DateTimeOffset.UtcNow,
            currentContract.EndDate,
            request.NewServiceId,
            ContractStatusIds.Renewed,
            currentContract.ClientId,
            currentContract.MethodPaymentId);

        _contractRepository.AddContract(replacementContract);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(replacementContract.ContractId, cancellationToken);
    }

    public async Task<ContractResponseDto> ChangePaymentMethodAsync(int contractId, ChangeContractPaymentMethodRequestDto request, CancellationToken cancellationToken = default)
    {
        var contract = await _contractRepository.GetForUpdateAsync(contractId, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el contrato {contractId}.");

        if (!contract.IsOperative())
        {
            throw new BusinessRuleException("Solo los contratos operativos pueden cambiar su forma de pago.");
        }

        var methodExists = await _contractRepository.MethodPaymentExistsAsync(request.MethodPaymentId, cancellationToken);
        if (!methodExists)
        {
            throw new NotFoundException($"No se encontró el método de pago {request.MethodPaymentId}.");
        }

        contract.ChangePaymentMethod(request.MethodPaymentId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(contract.ContractId, cancellationToken);
    }

    public async Task<ContractResponseDto> CancelAsync(int contractId, CancellationToken cancellationToken = default)
    {
        var contract = await _contractRepository.GetForUpdateAsync(contractId, cancellationToken)
            ?? throw new NotFoundException($"No se encontró el contrato {contractId}.");

        contract.Cancel(DateTimeOffset.UtcNow);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(contract.ContractId, cancellationToken);
    }

    private async Task EnsureRelationsAsync(int clientId, int serviceId, int methodPaymentId, CancellationToken cancellationToken)
    {
        if (!await _contractRepository.ClientExistsAsync(clientId, cancellationToken))
        {
            throw new NotFoundException($"No se encontró el cliente {clientId}.");
        }

        if (!await _contractRepository.ServiceExistsAsync(serviceId, cancellationToken))
        {
            throw new NotFoundException($"No se encontró el servicio {serviceId}.");
        }

        if (!await _contractRepository.MethodPaymentExistsAsync(methodPaymentId, cancellationToken))
        {
            throw new NotFoundException($"No se encontró el método de pago {methodPaymentId}.");
        }
    }
}
