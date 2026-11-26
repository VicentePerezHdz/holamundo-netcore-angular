using System.ComponentModel.DataAnnotations;

namespace HolaMundo.Api.Contracts;

// DTO (Data Transfer Objects): son los "sobres" que viajan en JSON entre Angular y .NET.
// Se usan records porque son inmutables y comparan por valor.

/// <summary>Lo que Angular envía al pedir un saludo.</summary>
public record SaludoRequest(
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
    string Nombre);

/// <summary>Lo que .NET responde con el saludo.</summary>
public record SaludoResponse(string Mensaje, DateTime FechaUtc);

/// <summary>Operación de la calculadora: suma, resta, multiplicacion, division.</summary>
public record CalculoRequest(
    decimal A,
    decimal B,
    [Required(ErrorMessage = "La operación es obligatoria.")] string Operacion);

public record CalculoResponse(decimal Resultado, string Expresion);

/// <summary>Elemento de un combobox (select): clave, texto visible y mensaje al elegirlo.</summary>
public record OpcionCatalogo(string Clave, string Texto, string Mensaje);
