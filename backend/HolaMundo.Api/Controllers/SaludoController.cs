using HolaMundo.Api.Contracts;
using HolaMundo.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HolaMundo.Api.Controllers;

/// <summary>
/// Controlador = puerta HTTP. Recibe la petición, delega en el servicio y devuelve la respuesta.
/// [ApiController] valida el body automáticamente y responde 400 si no cumple las DataAnnotations.
/// </summary>
[ApiController]
[Route("api/[controller]")] // => /api/saludo
public class SaludoController(ISaludoService saludoService) : ControllerBase
{
    /// <summary>GET /api/saludo : el "hola mundo" más simple.</summary>
    [HttpGet]
    public ActionResult<SaludoResponse> Get() => Ok(saludoService.Saludar("Mundo"));

    /// <summary>POST /api/saludo  body: { "nombre": "Vicente" }</summary>
    [HttpPost]
    public ActionResult<SaludoResponse> Post([FromBody] SaludoRequest request) =>
        Ok(saludoService.Saludar(request.Nombre));
}
