using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Viamatica.Application.Common;
using Viamatica.Application.DTOs.Cashes;
using Viamatica.Application.Interfaces;

namespace Viamatica.API.Controllers;

[Route("api/[controller]")]
[Authorize(Roles = RoleNames.AnyStaff)]
public sealed class CashesController : ApiControllerBase
{
    private readonly ICashManagementService _cashManagementService;

    public CashesController(ICashManagementService cashManagementService)
    {
        _cashManagementService = cashManagementService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<CashResponseDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _cashManagementService.GetAllAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CashResponseDto>> GetById(int id, CancellationToken cancellationToken)
        => Ok(await _cashManagementService.GetByIdAsync(id, cancellationToken));

    [HttpPost]
    [Authorize(Roles = RoleNames.AdminOrGestor)]
    public async Task<ActionResult<CashResponseDto>> Create([FromBody] CreateCashRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _cashManagementService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.CashId }, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = RoleNames.AdminOrGestor)]
    public async Task<ActionResult<CashResponseDto>> Update(int id, [FromBody] UpdateCashRequestDto request, CancellationToken cancellationToken)
        => Ok(await _cashManagementService.UpdateAsync(id, request, cancellationToken));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = RoleNames.AdminOrGestor)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _cashManagementService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPost("{cashId:int}/assignments/{userId:int}")]
    [Authorize(Roles = RoleNames.AdminOrGestor)]
    public async Task<IActionResult> AssignCashier(int cashId, int userId, CancellationToken cancellationToken)
    {
        await _cashManagementService.AssignCashierAsync(cashId, userId, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{cashId:int}/assignments/{userId:int}")]
    [Authorize(Roles = RoleNames.AdminOrGestor)]
    public async Task<IActionResult> UnassignCashier(int cashId, int userId, CancellationToken cancellationToken)
    {
        await _cashManagementService.UnassignCashierAsync(cashId, userId, cancellationToken);
        return NoContent();
    }

    [HttpPost("{cashId:int}/sessions/open")]
    [Authorize(Roles = $"{RoleNames.Cashier},{RoleNames.Administrator}")]
    public async Task<ActionResult<CashSessionResponseDto>> OpenSession(int cashId, CancellationToken cancellationToken)
        => Ok(await _cashManagementService.OpenSessionAsync(cashId, CurrentUserId, cancellationToken));

    [HttpPost("{cashId:int}/sessions/close")]
    [Authorize(Roles = $"{RoleNames.Cashier},{RoleNames.Administrator}")]
    public async Task<ActionResult<CashSessionResponseDto>> CloseSession(int cashId, CancellationToken cancellationToken)
        => Ok(await _cashManagementService.CloseSessionAsync(cashId, CurrentUserId, cancellationToken));
}
