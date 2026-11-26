using HolaMundo.Api.Contracts;

namespace HolaMundo.Api.Interfaces;

// Las interfaces son el "contrato" del servicio. El controlador depende de la
// interfaz, no de la clase concreta: así se puede cambiar la implementación o
// usar un doble en pruebas (inversión de dependencias, la D de SOLID).

public interface ISaludoService
{
    SaludoResponse Saludar(string nombre);
}

public interface ICalculadoraService
{
    CalculoResponse Calcular(CalculoRequest request);
}

public interface ICatalogoService
{
    Task<IReadOnlyList<OpcionCatalogo>> ObtenerLenguajesAsync(CancellationToken ct);
}
