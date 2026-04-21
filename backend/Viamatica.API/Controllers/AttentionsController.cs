using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Viamatica.Application.Common;
using Viamatica.Application.DTOs.Attentions;
using Viamatica.Application.Interfaces;

namespace Viamatica.API.Controllers;

[Route("api/[controller]")]
[Authorize(Roles = RoleNames.AnyStaff)]
public sealed class AttentionsController : ApiControllerBase
{
    private readonly IAttentionService _attentionService;

    public AttentionsController(IAttentionService attentionService)
    {
        _attentionService = attentionService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<AttentionResponseDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _attentionService.GetAllAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AttentionResponseDto>> GetById(int id, CancellationToken cancellationToken)
        => Ok(await _attentionService.GetByIdAsync(id, cancellationToken));

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyCollection<AttentionTypeResponseDto>>> GetTypes(CancellationToken cancellationToken)
        => Ok(await _attentionService.GetTypesAsync(cancellationToken));

    [HttpPost("start")]
    [Authorize(Roles = $"{RoleNames.Cashier},{RoleNames.Administrator}")]
    public async Task<ActionResult<AttentionResponseDto>> Start([FromBody] StartAttentionRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _attentionService.StartAsync(request, CurrentUserId, CurrentUserRole, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.AttentionId }, result);
    }

    [HttpPost("{id:int}/complete")]
    [Authorize(Roles = $"{RoleNames.Cashier},{RoleNames.Administrator}")]
    public async Task<ActionResult<AttentionResponseDto>> Complete(int id, [FromBody] CloseAttentionRequestDto request, CancellationToken cancellationToken)
        => Ok(await _attentionService.CompleteAsync(id, request, CurrentUserId, CurrentUserRole, cancellationToken));

    [HttpPost("{id:int}/cancel")]
    [Authorize(Roles = $"{RoleNames.Cashier},{RoleNames.Administrator}")]
    public async Task<ActionResult<AttentionResponseDto>> Cancel(int id, [FromBody] CloseAttentionRequestDto request, CancellationToken cancellationToken)
        => Ok(await _attentionService.CancelAsync(id, request, CurrentUserId, CurrentUserRole, cancellationToken));
}
