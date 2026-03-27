using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace IncidentManager.API.Middleware;

/// <summary>
/// Captura excepciones no controladas y las convierte en ProblemDetails.
/// Registrar en Program.cs con app.UseExceptionHandler().
/// </summary>
internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context, Exception exception, CancellationToken ct)
    {
        logger.LogError(exception, "Excepción no controlada: {Message}", exception.Message);
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title  = "Error interno del servidor",
            Detail = exception.Message
        }, ct);
        return true;
    }
}
