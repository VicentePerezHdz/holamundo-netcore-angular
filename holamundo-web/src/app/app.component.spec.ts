import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { HolaMundoApiService } from './services/hola-mundo-api.service';

/** Prueba del servicio sin backend: HttpTestingController simula las respuestas de .NET. */
describe('HolaMundoApiService', () => {
  let api: HolaMundoApiService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [provideHttpClient(), provideHttpClientTesting()] });
    api = TestBed.inject(HolaMundoApiService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('saludar hace POST /api/saludo con el nombre', () => {
    api.saludar('Vicente').subscribe(r => expect(r.mensaje).toContain('Vicente'));
    const req = http.expectOne('http://localhost:5080/api/saludo');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ nombre: 'Vicente' });
    req.flush({ mensaje: '¡Hola, Vicente!', fechaUtc: '2026-01-01T00:00:00Z' });
  });

  it('traduce el ProblemDetails de .NET a un mensaje', () => {
    api.calcular({ a: 1, b: 0, operacion: 'division' }).subscribe({
      error: (e: Error) => expect(e.message).toBe('No se puede dividir entre cero.')
    });
    http.expectOne('http://localhost:5080/api/calculadora')
      .flush({ title: 'Regla de negocio', detail: 'No se puede dividir entre cero.' }, { status: 400, statusText: 'Bad Request' });
  });
});
