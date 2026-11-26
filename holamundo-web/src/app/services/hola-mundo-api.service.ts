import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  CalculoRequest, CalculoResponse, OpcionCatalogo, ProblemDetails, SaludoResponse
} from '../models/contratos';

/**
 * Único punto donde Angular habla con .NET. Los componentes nunca usan HttpClient directo:
 * llaman a este servicio (equivalente a ISaludoService/ICalculadoraService del backend).
 * providedIn: 'root' => Angular crea una sola instancia (singleton) y la inyecta donde se pida.
 */
@Injectable({ providedIn: 'root' })
export class HolaMundoApiService {
  private readonly http = inject(HttpClient);
  private readonly base = environment.apiUrl;

  saludar(nombre: string): Observable<SaludoResponse> {
    return this.http.post<SaludoResponse>(`${this.base}/saludo`, { nombre })
      .pipe(catchError(HolaMundoApiService.aMensaje));
  }

  calcular(req: CalculoRequest): Observable<CalculoResponse> {
    return this.http.post<CalculoResponse>(`${this.base}/calculadora`, req)
      .pipe(catchError(HolaMundoApiService.aMensaje));
  }

  lenguajes(): Observable<OpcionCatalogo[]> {
    return this.http.get<OpcionCatalogo[]>(`${this.base}/catalogos/lenguajes`)
      .pipe(catchError(HolaMundoApiService.aMensaje));
  }

  /** Traduce cualquier error HTTP a un texto legible usando el ProblemDetails de .NET. */
  static aMensaje(err: HttpErrorResponse): Observable<never> {
    if (err.status === 0) {
      return throwError(() => new Error('No hay conexión con la API. ¿Está corriendo "dotnet run" en el puerto 5080?'));
    }
    const p = err.error as ProblemDetails | null;
    const validacion = p?.errors ? Object.values(p.errors).flat().join(' ') : '';
    return throwError(() => new Error(validacion || p?.detail || p?.title || `Error ${err.status}`));
  }
}
