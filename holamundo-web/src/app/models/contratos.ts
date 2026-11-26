// Espejo en TypeScript de los records de C# (HolaMundo.Api/Contracts/Contratos.cs).
// .NET serializa en camelCase: Nombre -> nombre, FechaUtc -> fechaUtc.

export interface SaludoRequest { nombre: string; }
export interface SaludoResponse { mensaje: string; fechaUtc: string; }

export type Operacion = 'suma' | 'resta' | 'multiplicacion' | 'division';
export interface CalculoRequest { a: number; b: number; operacion: Operacion; }
export interface CalculoResponse { resultado: number; expresion: string; }

export interface OpcionCatalogo { clave: string; texto: string; mensaje: string; }

/** Formato de error estándar que devuelve .NET (ProblemDetails, RFC 7807). */
export interface ProblemDetails {
  title?: string;
  detail?: string;
  status?: number;
  errors?: Record<string, string[]>; // solo en errores de validación (400 automático de [ApiController])
}
