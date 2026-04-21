using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Viamatica.Application.DTOs;
using Viamatica.Application.Interfaces;

namespace Viamatica.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var response = await _authService.LoginAsync(request, cancellationToken);

        if (response is null)
        {
            _logger.LogWarning(
                "Failed login attempt for {UserNameOrEmail} from {RemoteIp}",
                request.UserNameOrEmail.Trim(),
                HttpContext.Connection.RemoteIpAddress?.ToString());
            return Unauthorized(new { message = "Credenciales inválidas." });
        }

        _logger.LogInformation(
            "Successful login for {UserName} with role {Role} from {RemoteIp}",
            response.UserName,
            response.Role,
            HttpContext.Connection.RemoteIpAddress?.ToString());
        return Ok(response);
    }
}
