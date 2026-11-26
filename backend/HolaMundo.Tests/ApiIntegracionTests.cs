using System.Net;
using System.Net.Http.Json;
using HolaMundo.Api.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HolaMundo.Tests;

/// <summary>
/// Prueba de integración: levanta la API en memoria (WebApplicationFactory) y le hace
/// peticiones HTTP reales, igual que lo haría Angular.
/// </summary>
public class ApiIntegracionTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _http = factory.CreateClient();

    [Fact]
    public async Task PostSaludo_DevuelveMensajeConNombre()
    {
        var resp = await _http.PostAsJsonAsync("/api/saludo", new SaludoRequest("Vicente"));
        resp.EnsureSuccessStatusCode();
        var saludo = await resp.Content.ReadFromJsonAsync<SaludoResponse>();
        Assert.Contains("Vicente", saludo!.Mensaje);
    }

    [Fact]
    public async Task PostSaludo_NombreVacio_Devuelve400()
    {
        var resp = await _http.PostAsJsonAsync("/api/saludo", new SaludoRequest(""));
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task PostCalculadora_DivisionEntreCero_DevuelveProblemDetails()
    {
        var resp = await _http.PostAsJsonAsync("/api/calculadora", new CalculoRequest(1, 0, "division"));
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        var problema = await resp.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("No se puede dividir entre cero.", problema!.Detail);
    }

    [Fact]
    public async Task GetLenguajes_DevuelveOpcionesDelCombo()
    {
        var opciones = await _http.GetFromJsonAsync<List<OpcionCatalogo>>("/api/catalogos/lenguajes");
        Assert.Equal(5, opciones!.Count);
    }
}
