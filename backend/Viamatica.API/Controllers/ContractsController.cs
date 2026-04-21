using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Viamatica.Application.Common;
using Viamatica.Application.DTOs.Contracts;
using Viamatica.Application.Interfaces;

namespace Viamatica.API.Controllers;

[Route("api/[controller]")]
[Authorize(Roles = RoleNames.AnyStaff)]
public sealed class ContractsController : ApiControllerBase
{
    private readonly IContractService _contractService;

    public ContractsController(IContractService contractService)
    {
        _contractService = contractService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ContractResponseDto>>> GetAll([FromQuery] int? clientId, CancellationToken cancellationToken)
        => Ok(await _contractService.GetAllAsync(clientId, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ContractResponseDto>> GetById(int id, CancellationToken cancellationToken)
        => Ok(await _contractService.GetByIdAsync(id, cancellationToken));

    [HttpGet("payment-methods")]
    public async Task<ActionResult<IReadOnlyCollection<PaymentMethodResponseDto>>> GetPaymentMethods(CancellationToken cancellationToken)
        => Ok(await _contractService.GetPaymentMethodsAsync(cancellationToken));

    [HttpPost]
    public async Task<ActionResult<ContractResponseDto>> Create([FromBody] CreateContractRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _contractService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.ContractId }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ContractResponseDto>> Update(int id, [FromBody] UpdateContractRequestDto request, CancellationToken cancellationToken)
        => Ok(await _contractService.UpdateAsync(id, request, cancellationToken));

    [HttpPost("{id:int}/payments")]
    public async Task<ActionResult<PaymentResponseDto>> RegisterPayment(int id, [FromBody] CreatePaymentRequestDto request, CancellationToken cancellationToken)
        => Ok(await _contractService.RegisterPaymentAsync(id, request, cancellationToken));

    [HttpPost("{id:int}/change-service")]
    public async Task<ActionResult<ContractResponseDto>> ChangeService(int id, [FromBody] ChangeContractServiceRequestDto request, CancellationToken cancellationToken)
        => Ok(await _contractService.ChangeServiceAsync(id, request, cancellationToken));

    [HttpPost("{id:int}/change-payment-method")]
    public async Task<ActionResult<ContractResponseDto>> ChangePaymentMethod(int id, [FromBody] ChangeContractPaymentMethodRequestDto request, CancellationToken cancellationToken)
        => Ok(await _contractService.ChangePaymentMethodAsync(id, request, cancellationToken));

    [HttpPost("{id:int}/cancel")]
    public async Task<ActionResult<ContractResponseDto>> Cancel(int id, CancellationToken cancellationToken)
        => Ok(await _contractService.CancelAsync(id, cancellationToken));
}
