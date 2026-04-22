using Viamatica.Application.DTOs.Contracts;
using Viamatica.Domain.Entities;

namespace Viamatica.Application.Interfaces;

public interface IContractRepository
{
    Task<IReadOnlyCollection<ContractResponseDto>> GetAllAsync(int? clientId, CancellationToken cancellationToken = default);
    Task<ContractResponseDto?> GetByIdAsync(int contractId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<PaymentMethodResponseDto>> GetPaymentMethodsAsync(CancellationToken cancellationToken = default);
    Task<Contract?> GetForUpdateAsync(int contractId, CancellationToken cancellationToken = default);
    Task<bool> ClientExistsAsync(int clientId, CancellationToken cancellationToken = default);
    Task<bool> ServiceExistsAsync(int serviceId, CancellationToken cancellationToken = default);
    Task<bool> MethodPaymentExistsAsync(int methodPaymentId, CancellationToken cancellationToken = default);
    Task<bool> ContractStatusExistsAsync(string statusId, CancellationToken cancellationToken = default);
    Task<PaymentResponseDto?> GetPaymentByIdAsync(int paymentId, CancellationToken cancellationToken = default);
    void AddContract(Contract contract);
    void AddPayment(Payment payment);
}
