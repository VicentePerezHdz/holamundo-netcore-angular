using HolaMundo.Api.Contracts;
using HolaMundo.Api.Exceptions;
using HolaMundo.Api.Interfaces;

namespace HolaMundo.Api.Services;

/// <summary>Regla de negocio de la calculadora. No sabe nada de HTTP: recibe datos y devuelve datos.</summary>
public class CalculadoraService : ICalculadoraService
{
    public CalculoResponse Calcular(CalculoRequest request)
    {
        var (a, b) = (request.A, request.B);

        // switch expression de C#: cada operación devuelve (resultado, símbolo).
        var (resultado, simbolo) = request.Operacion.ToLowerInvariant() switch
        {
            "suma" => (a + b, "+"),
            "resta" => (a - b, "-"),
            "multiplicacion" => (a * b, "×"),
            "division" when b == 0 => throw new ReglaNegocioException("No se puede dividir entre cero."),
            "division" => (a / b, "÷"),
            _ => throw new ReglaNegocioException($"Operación '{request.Operacion}' no soportada.")
        };

        var redondeado = decimal.Round(resultado, 4);
        return new CalculoResponse(redondeado, $"{a} {simbolo} {b} = {redondeado}");
    }
}
