namespace HolaMundo.Api.Exceptions;

/// <summary>
/// Error esperado de negocio (p. ej. dividir entre cero). El manejador global
/// lo convierte en HTTP 400 con formato ProblemDetails.
/// </summary>
public class ReglaNegocioException(string mensaje) : Exception(mensaje);
