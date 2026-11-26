using HolaMundo.Api.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HolaMundo.Api.Middleware;

/// <summary>
/// IExceptionHandler (nuevo en .NET 8): un solo lugar convierte excepciones en respuestas
/// ProblemDetails (RFC 7807). Así todos los errores de la API tienen el mismo formato.
/// </summary>
public class ManejadorErroresGlobal(ILogger<ManejadorErroresGlobal> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext http, Exception ex, CancellationToken ct)
    {
        var (status, titulo) = ex switch
        {
            ReglaNegocioException => (StatusCodes.Status400BadRequest, "Regla de negocio"),
            OperationCanceledException => (499, "Petición cancelada por el cliente"),
            _ => (StatusCodes.Status500InternalServerError, "Error inesperado")
        };

        if (status == StatusCodes.Status500InternalServerError) logger.LogError(ex, "Error no controlado");

        http.Response.StatusCode = status;
        await http.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status,
            Title = titulo,
            // Nunca exponer el detalle técnico de un 500 al cliente.
            Detail = status == StatusCodes.Status500InternalServerError ? "Ocurrió un error. Revisa el log del servidor." : ex.Message,
            Instance = http.Request.Path
        }, ct);
        return true;
    }
}
