using System.Text.Json;
using Viamatica.Application.Common;

namespace Viamatica.API.Middleware;

public sealed class ApiExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionMiddleware> _logger;

    public ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            var statusCode = exception switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                ConflictException => StatusCodes.Status409Conflict,
                ForbiddenOperationException => StatusCodes.Status403Forbidden,
                BusinessRuleException => StatusCodes.Status400BadRequest,
                ArgumentException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            if (statusCode >= StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(exception, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
            }
            else
            {
                _logger.LogWarning(exception, "Handled exception {StatusCode} for {Method} {Path}", statusCode, context.Request.Method, context.Request.Path);
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var payload = new
            {
                message = exception.Message,
                type = exception.GetType().Name
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}
