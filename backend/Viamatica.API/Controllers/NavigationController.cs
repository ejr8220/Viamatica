using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Viamatica.Application.Common;
using Viamatica.Application.DTOs.Navigation;
using Viamatica.Application.Interfaces;

namespace Viamatica.API.Controllers;

[Route("api/[controller]")]
[Authorize(Roles = RoleNames.AnyStaff)]
public sealed class NavigationController : ApiControllerBase
{
    private readonly INavigationService _navigationService;

    public NavigationController(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    [HttpGet("menu")]
    public async Task<ActionResult<IReadOnlyCollection<NavigationMenuItemDto>>> GetMenu(CancellationToken cancellationToken)
        => Ok(await _navigationService.GetMenuAsync(CurrentUserRole, cancellationToken));
}
