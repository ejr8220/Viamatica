using Viamatica.Application.DTOs.Contracts;

namespace Viamatica.Application.Interfaces;

public interface IContractService
{
    Task<IReadOnlyCollection<ContractResponseDto>> GetAllAsync(int? clientId, CancellationToken cancellationToken = default);
    Task<ContractResponseDto> GetByIdAsync(int contractId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<PaymentMethodResponseDto>> GetPaymentMethodsAsync(CancellationToken cancellationToken = default);
    Task<ContractResponseDto> CreateAsync(CreateContractRequestDto request, CancellationToken cancellationToken = default);
    Task<ContractResponseDto> UpdateAsync(int contractId, UpdateContractRequestDto request, CancellationToken cancellationToken = default);
    Task<PaymentResponseDto> RegisterPaymentAsync(int contractId, CreatePaymentRequestDto request, CancellationToken cancellationToken = default);
    Task<ContractResponseDto> ChangeServiceAsync(int contractId, ChangeContractServiceRequestDto request, CancellationToken cancellationToken = default);
    Task<ContractResponseDto> ChangePaymentMethodAsync(int contractId, ChangeContractPaymentMethodRequestDto request, CancellationToken cancellationToken = default);
    Task<ContractResponseDto> CancelAsync(int contractId, CancellationToken cancellationToken = default);
}
