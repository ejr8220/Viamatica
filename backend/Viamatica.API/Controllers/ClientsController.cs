using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Viamatica.Application.Common;
using Viamatica.Application.DTOs.Clients;
using Viamatica.Application.Interfaces;

namespace Viamatica.API.Controllers;

[Route("api/[controller]")]
[Authorize(Roles = RoleNames.AnyStaff)]
public sealed class ClientsController : ApiControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ClientResponseDto>>> GetAll([FromQuery] string? identification, CancellationToken cancellationToken)
        => Ok(await _clientService.GetAllAsync(identification, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClientResponseDto>> GetById(int id, CancellationToken cancellationToken)
        => Ok(await _clientService.GetByIdAsync(id, cancellationToken));

    [HttpPost]
    public async Task<ActionResult<ClientResponseDto>> Create([FromBody] CreateClientRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _clientService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.ClientId }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ClientResponseDto>> Update(int id, [FromBody] UpdateClientRequestDto request, CancellationToken cancellationToken)
        => Ok(await _clientService.UpdateAsync(id, request, cancellationToken));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _clientService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
