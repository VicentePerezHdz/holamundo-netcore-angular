using HolaMundo.Api.Contracts;
using HolaMundo.Api.Exceptions;
using HolaMundo.Api.Services;

namespace HolaMundo.Tests;

/// <summary>Prueba unitaria: el servicio se prueba solo, sin HTTP ni servidor.</summary>
public class CalculadoraServiceTests
{
    private readonly CalculadoraService _sut = new(); // sut = system under test

    [Theory]
    [InlineData(10, 5, "suma", 15)]
    [InlineData(10, 5, "resta", 5)]
    [InlineData(10, 5, "multiplicacion", 50)]
    [InlineData(10, 4, "division", 2.5)]
    public void Calcular_OperacionValida_DevuelveResultado(decimal a, decimal b, string op, decimal esperado)
    {
        var respuesta = _sut.Calcular(new CalculoRequest(a, b, op));
        Assert.Equal(esperado, respuesta.Resultado);
    }

    [Fact]
    public void Calcular_DivisionEntreCero_LanzaReglaNegocio()
    {
        var ex = Assert.Throws<ReglaNegocioException>(() => _sut.Calcular(new CalculoRequest(1, 0, "division")));
        Assert.Equal("No se puede dividir entre cero.", ex.Message);
    }
}
