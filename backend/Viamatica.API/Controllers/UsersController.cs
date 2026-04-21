using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Viamatica.Application.Common;
using Viamatica.Application.DTOs.Users;
using Viamatica.Application.Interfaces;

namespace Viamatica.API.Controllers;

[Route("api/[controller]")]
[Authorize(Roles = RoleNames.AdminOrGestor)]
public sealed class UsersController : ApiControllerBase
{
    private readonly IUserManagementService _userManagementService;

    public UsersController(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<UserResponseDto>>> GetAll([FromQuery] bool pendingOnly, CancellationToken cancellationToken)
        => Ok(await _userManagementService.GetAllAsync(pendingOnly, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponseDto>> GetById(int id, CancellationToken cancellationToken)
        => Ok(await _userManagementService.GetByIdAsync(id, cancellationToken));

    [HttpGet("active-report")]
    public async Task<ActionResult<IReadOnlyCollection<ActiveUserReportDto>>> GetActiveReport(CancellationToken cancellationToken)
        => Ok(await _userManagementService.GetActiveReportAsync(cancellationToken));

    [HttpPost]
    public async Task<ActionResult<UserResponseDto>> Create([FromBody] CreateUserRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _userManagementService.CreateAsync(request, CurrentUserId, CurrentUserRole, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.UserId }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserResponseDto>> Update(int id, [FromBody] UpdateUserRequestDto request, CancellationToken cancellationToken)
        => Ok(await _userManagementService.UpdateAsync(id, request, CurrentUserRole, cancellationToken));

    [HttpPost("{id:int}/approve")]
    [Authorize(Roles = RoleNames.Administrator)]
    public async Task<IActionResult> Approve(int id, CancellationToken cancellationToken)
    {
        await _userManagementService.ApproveAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _userManagementService.DeleteAsync(id, CurrentUserRole, cancellationToken);
        return NoContent();
    }
}
