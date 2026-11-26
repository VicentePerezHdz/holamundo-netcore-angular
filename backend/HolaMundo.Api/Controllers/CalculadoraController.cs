using HolaMundo.Api.Contracts;
using HolaMundo.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HolaMundo.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // => /api/calculadora
public class CalculadoraController(ICalculadoraService calculadora) : ControllerBase
{
    /// <summary>POST /api/calculadora  body: { "a": 10, "b": 5, "operacion": "suma" }</summary>
    [HttpPost]
    [ProducesResponseType<CalculoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<CalculoResponse> Calcular([FromBody] CalculoRequest request) =>
        Ok(calculadora.Calcular(request));
}
