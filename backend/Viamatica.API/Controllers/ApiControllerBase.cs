using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Viamatica.API.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("El token no contiene el identificador del usuario."));

    protected string CurrentUserRole =>
        User.FindFirstValue(ClaimTypes.Role)
        ?? throw new InvalidOperationException("El token no contiene el rol del usuario.");
}
