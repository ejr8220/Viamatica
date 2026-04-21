using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Viamatica.Application.Common;
using Viamatica.Application.DTOs.Turns;
using Viamatica.Application.Interfaces;

namespace Viamatica.API.Controllers;

[Route("api/[controller]")]
[Authorize(Roles = RoleNames.AnyStaff)]
public sealed class TurnsController : ApiControllerBase
{
    private readonly ITurnService _turnService;

    public TurnsController(ITurnService turnService)
    {
        _turnService = turnService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<TurnResponseDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _turnService.GetAllAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TurnResponseDto>> GetById(int id, CancellationToken cancellationToken)
        => Ok(await _turnService.GetByIdAsync(id, cancellationToken));

    [HttpPost]
    [Authorize(Roles = RoleNames.AdminOrGestor)]
    public async Task<ActionResult<TurnResponseDto>> Create([FromBody] CreateTurnRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _turnService.CreateAsync(request, CurrentUserId, CurrentUserRole, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.TurnId }, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = RoleNames.AdminOrGestor)]
    public async Task<ActionResult<TurnResponseDto>> Update(int id, [FromBody] UpdateTurnRequestDto request, CancellationToken cancellationToken)
        => Ok(await _turnService.UpdateAsync(id, request, CurrentUserId, CurrentUserRole, cancellationToken));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = RoleNames.AdminOrGestor)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _turnService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
