using HolaMundo.Api.Contracts;
using HolaMundo.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HolaMundo.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // => /api/catalogos
public class CatalogosController(ICatalogoService catalogos) : ControllerBase
{
    /// <summary>GET /api/catalogos/lenguajes : opciones del combobox. ASP.NET pasa el CancellationToken solo.</summary>
    [HttpGet("lenguajes")]
    public async Task<ActionResult<IReadOnlyList<OpcionCatalogo>>> Lenguajes(CancellationToken ct) =>
        Ok(await catalogos.ObtenerLenguajesAsync(ct));
}
