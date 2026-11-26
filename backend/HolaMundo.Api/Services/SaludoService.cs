using HolaMundo.Api.Contracts;
using HolaMundo.Api.Interfaces;

namespace HolaMundo.Api.Services;

/// <summary>Arma el mensaje de saludo. TimeProvider se inyecta para poder fijar la hora en pruebas.</summary>
public class SaludoService(TimeProvider reloj) : ISaludoService
{
    public SaludoResponse Saludar(string nombre)
    {
        var limpio = nombre.Trim();
        return new SaludoResponse($"¡Hola, {limpio}! Este mensaje viene de ASP.NET Core 8.", reloj.GetUtcNow().UtcDateTime);
    }
}
