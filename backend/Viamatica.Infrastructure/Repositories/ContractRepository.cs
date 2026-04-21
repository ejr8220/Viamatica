using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Viamatica.Application.DTOs.Contracts;
using Viamatica.Application.Interfaces;
using Viamatica.Domain.Entities;
using Viamatica.Infrastructure.Data;

namespace Viamatica.Infrastructure.Repositories;

public sealed class ContractRepository : IContractRepository
{
    private readonly ViamaticaDbContext _dbContext;

    public ContractRepository(ViamaticaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<ContractResponseDto>> GetAllAsync(int? clientId, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Contracts
            .AsNoTracking()
            .Include(contract => contract.Service)
            .Include(contract => contract.Status)
            .Include(contract => contract.Client)
            .Include(contract => contract.MethodPayment)
            .Include(contract => contract.Payments)
            .AsQueryable();

        if (clientId.HasValue)
        {
            query = query.Where(contract => contract.ClientId == clientId.Value);
        }

        return await query
            .OrderByDescending(contract => contract.ContractId)
            .Select(MapContract())
            .ToListAsync(cancellationToken);
    }

    public Task<ContractResponseDto?> GetByIdAsync(int contractId, CancellationToken cancellationToken = default)
        => _dbContext.Contracts
            .AsNoTracking()
            .Include(entity => entity.Service)
            .Include(entity => entity.Status)
            .Include(entity => entity.Client)
            .Include(entity => entity.MethodPayment)
            .Include(entity => entity.Payments)
            .Where(entity => entity.ContractId == contractId)
            .Select(MapContract())
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyCollection<PaymentMethodResponseDto>> GetPaymentMethodsAsync(CancellationToken cancellationToken = default)
        => await _dbContext.MethodPayments
            .AsNoTracking()
            .OrderBy(method => method.MethodPaymentId)
            .Select(method => new PaymentMethodResponseDto
            {
                MethodPaymentId = method.MethodPaymentId,
                Description = method.Description
            })
            .ToListAsync(cancellationToken);

    public Task<Contract?> GetForUpdateAsync(int contractId, CancellationToken cancellationToken = default)
        => _dbContext.Contracts.FirstOrDefaultAsync(entity => entity.ContractId == contractId, cancellationToken);

    public Task<bool> ClientExistsAsync(int clientId, CancellationToken cancellationToken = default)
        => _dbContext.Clients.AnyAsync(client => client.ClientId == clientId, cancellationToken);

    public Task<bool> ServiceExistsAsync(int serviceId, CancellationToken cancellationToken = default)
        => _dbContext.Services.AnyAsync(service => service.ServiceId == serviceId, cancellationToken);

    public Task<bool> MethodPaymentExistsAsync(int methodPaymentId, CancellationToken cancellationToken = default)
        => _dbContext.MethodPayments.AnyAsync(method => method.MethodPaymentId == methodPaymentId, cancellationToken);

    public Task<PaymentResponseDto?> GetPaymentByIdAsync(int paymentId, CancellationToken cancellationToken = default)
        => _dbContext.Payments
            .AsNoTracking()
            .Where(entity => entity.PaymentId == paymentId)
            .Select(entity => new PaymentResponseDto
            {
                PaymentId = entity.PaymentId,
                ContractId = entity.ContractId,
                ClientId = entity.ClientId,
                Amount = entity.Amount,
                Description = entity.Description,
                PaymentDate = entity.PaymentDate
            })
            .FirstOrDefaultAsync(cancellationToken);

    public void AddContract(Contract contract) => _dbContext.Contracts.Add(contract);

    public void AddPayment(Payment payment) => _dbContext.Payments.Add(payment);

    private static Expression<Func<Contract, ContractResponseDto>> MapContract()
    {
        return contract => new ContractResponseDto
        {
            ContractId = contract.ContractId,
            StartDate = contract.StartDate,
            EndDate = contract.EndDate,
            ServiceId = contract.ServiceId,
            ServiceName = contract.Service.ServiceName,
            StatusId = contract.StatusId,
            StatusDescription = contract.Status.Description,
            ClientId = contract.ClientId,
            ClientName = contract.Client.Name + " " + contract.Client.LastName,
            MethodPaymentId = contract.MethodPaymentId,
            MethodPaymentDescription = contract.MethodPayment.Description,
            Payments = contract.Payments
                .OrderByDescending(payment => payment.PaymentDate)
                .Select(payment => new PaymentResponseDto
                {
                    PaymentId = payment.PaymentId,
                    ContractId = payment.ContractId,
                    ClientId = payment.ClientId,
                    Amount = payment.Amount,
                    Description = payment.Description,
                    PaymentDate = payment.PaymentDate
                })
                .ToList()
        };
    }
}
