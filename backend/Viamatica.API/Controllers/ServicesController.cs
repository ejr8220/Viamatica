using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Viamatica.Application.Common;
using Viamatica.Application.DTOs.Services;
using Viamatica.Application.Interfaces;

namespace Viamatica.API.Controllers;

[Route("api/[controller]")]
[Authorize(Roles = RoleNames.AnyStaff)]
public sealed class ServicesController : ApiControllerBase
{
    private readonly IServiceCatalogService _serviceCatalogService;

    public ServicesController(IServiceCatalogService serviceCatalogService)
    {
        _serviceCatalogService = serviceCatalogService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ServiceResponseDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _serviceCatalogService.GetAllAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ServiceResponseDto>> GetById(int id, CancellationToken cancellationToken)
        => Ok(await _serviceCatalogService.GetByIdAsync(id, cancellationToken));

    [HttpPost]
    [Authorize(Roles = RoleNames.AdminOrGestor)]
    public async Task<ActionResult<ServiceResponseDto>> Create([FromBody] CreateServiceRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _serviceCatalogService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.ServiceId }, result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = RoleNames.AdminOrGestor)]
    public async Task<ActionResult<ServiceResponseDto>> Update(int id, [FromBody] UpdateServiceRequestDto request, CancellationToken cancellationToken)
        => Ok(await _serviceCatalogService.UpdateAsync(id, request, cancellationToken));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = RoleNames.AdminOrGestor)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _serviceCatalogService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
