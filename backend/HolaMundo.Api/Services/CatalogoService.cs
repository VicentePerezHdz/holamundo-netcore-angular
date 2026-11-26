using HolaMundo.Api.Contracts;
using HolaMundo.Api.Interfaces;

namespace HolaMundo.Api.Services;

/// <summary>
/// Simula una consulta a base de datos para llenar el combobox. Es async y respeta el
/// CancellationToken: si el navegador cancela la petición, .NET deja de trabajar.
/// </summary>
public class CatalogoService : ICatalogoService
{
    private static readonly IReadOnlyList<OpcionCatalogo> Lenguajes =
    [
        new("csharp", "C# / .NET 8", "Backend: controladores, servicios e inyección de dependencias."),
        new("typescript", "TypeScript / Angular 17", "Frontend: componentes standalone, servicios y HttpClient."),
        new("kotlin", "Kotlin / Android", "Móvil: ViewModel, coroutines y Dagger 2 (arquitectura AC Móvil)."),
        new("python", "Python / FastAPI", "Scripts, datos, PySpark en Databricks y APIs ligeras."),
        new("react", "React / TypeScript", "Frontend con hooks; se combina con un backend .NET.")
    ];

    public async Task<IReadOnlyList<OpcionCatalogo>> ObtenerLenguajesAsync(CancellationToken ct)
    {
        await Task.Delay(150, ct); // simula la latencia de una consulta real
        return Lenguajes;
    }
}
