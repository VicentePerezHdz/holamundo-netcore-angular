# Documento de transición: Hola Mundo Angular 17 + .NET 8

> Manual para entender qué se construyó, cómo se comunican Angular y .NET, y qué hace cada clase, método e interfaz.
> Fecha: 2026-11-23 · Ruta: `C:\HolaMundo\01-NetCore-Angular`

## 1. Qué hace la aplicación

Una página Angular con tres tarjetas. Cada una llama a la API de .NET 8:

| Tarjeta | Control en pantalla | Llamada HTTP | Qué hace .NET |
|---|---|---|---|
| Saludo | input de texto + botón | `POST /api/saludo` `{ "nombre": "Vicente" }` | valida el nombre y arma el mensaje |
| Calculadora | 2 inputs numéricos + select de operación | `POST /api/calculadora` `{ "a": 10, "b": 5, "operacion": "suma" }` | calcula; si divides entre 0 responde 400 |
| Combobox | select llenado desde la API | `GET /api/catalogos/lenguajes` | devuelve la lista de opciones; Angular muestra el mensaje de la opción elegida |

## 2. Cómo correrlo

```powershell
# Terminal 1: backend
cd C:\HolaMundo\01-NetCore-Angular\backend\HolaMundo.Api
dotnet run --launch-profile http        # http://localhost:5080  (Swagger: http://localhost:5080/swagger)

# Terminal 2: frontend
cd C:\HolaMundo\01-NetCore-Angular\holamundo-web
npm start                                # http://localhost:4200
```

Pruebas:

```powershell
cd C:\HolaMundo\01-NetCore-Angular\backend;      dotnet test                            # 9 pruebas xUnit
cd C:\HolaMundo\01-NetCore-Angular\holamundo-web; npx ng test --watch=false --browsers=ChromeHeadless   # 2 pruebas Jasmine
```

Nota: tu máquina tiene el SDK de .NET 10 y el runtime de .NET 8. El proyecto apunta a `net8.0`, así que se compila con el SDK 10 y corre sobre el runtime 8, que es lo normal.

## 3. Arquitectura

```
┌────────────────────────── Navegador (localhost:4200) ──────────────────────────┐
│ AppComponent                                                                   │
│   ├─ SaludoComponent ──────┐                                                   │
│   ├─ CalculadoraComponent ─┼──► HolaMundoApiService ──► HttpClient ──┐         │
│   └─ ComboLenguajesComponent┘    (único que conoce las URLs)          │         │
└───────────────────────────────────────────────────────────────────────┼─────────┘
                                         JSON por HTTP + CORS          │
┌──────────────────────── ASP.NET Core 8 (localhost:5080) ─────────────▼─────────┐
│ Middlewares: ExceptionHandler → Swagger → CORS → Controllers                   │
│   SaludoController ───────► ISaludoService ─────► SaludoService                │
│   CalculadoraController ──► ICalculadoraService ► CalculadoraService           │
│   CatalogosController ────► ICatalogoService ───► CatalogoService (async)      │
│ ManejadorErroresGlobal: convierte excepciones en ProblemDetails (JSON)         │
└────────────────────────────────────────────────────────────────────────────────┘
```

Es una arquitectura **en capas** sencilla:

- **Presentación (Angular)**: componentes que solo pintan y reaccionan a eventos.
- **Acceso a datos del front**: `HolaMundoApiService`, el único que hace HTTP.
- **API (Controllers)**: traducen HTTP ↔ objetos C#. No tienen lógica de negocio.
- **Negocio (Services)**: las reglas. No saben nada de HTTP, por eso se prueban fácil.
- **Contratos (DTO)**: los records que viajan como JSON. En Angular existe el espejo en `models/contratos.ts`.

En un proyecto empresarial esto crece a **Clean Architecture** con proyectos separados (`Api`, `Application`, `Domain`, `Infrastructure`). El proyecto `03-Portafolio-Pedidos` lo muestra.

## 4. Recorrido de una petición (Saludo)

1. El usuario escribe "Vicente" y da clic en **Saludar** → `SaludoComponent.saludar()`.
2. El componente llama `HolaMundoApiService.saludar('Vicente')`, que hace `POST http://localhost:5080/api/saludo` con `{ "nombre": "Vicente" }`.
3. El navegador ve otro origen (puerto 4200 vs 5080) y revisa CORS. .NET responde con `Access-Control-Allow-Origin: http://localhost:4200` porque `Program.cs` lo permite.
4. ASP.NET enruta a `SaludoController.Post`. `[ApiController]` deserializa el JSON a `SaludoRequest` y valida `[Required]` y `[StringLength]`. Si falla, responde **400** sin entrar al método.
5. El controlador llama `ISaludoService.Saludar()`. El contenedor de DI ya le inyectó `SaludoService`.
6. Se devuelve `SaludoResponse`, que .NET serializa en **camelCase**: `{ "mensaje": "...", "fechaUtc": "..." }`.
7. El `Observable` de Angular emite. El componente hace `mensaje.set(...)` y la vista se actualiza (signals).

Si algo falla (división entre cero, por ejemplo), el servicio lanza `ReglaNegocioException`, `ManejadorErroresGlobal` la convierte en `400 + ProblemDetails`, y `HolaMundoApiService.aMensaje()` saca el `detail` para mostrarlo.

## 5. Inventario de clases, interfaces y métodos

### Backend (`backend/HolaMundo.Api`)

| Archivo | Tipo | Miembros | Responsabilidad |
|---|---|---|---|
| `Program.cs` | punto de entrada | — | Registra servicios en DI, CORS, Swagger, el manejador de errores y el pipeline de middlewares |
| `Contracts/Contratos.cs` | records (DTO) | `SaludoRequest`, `SaludoResponse`, `CalculoRequest`, `CalculoResponse`, `OpcionCatalogo` | Forma del JSON de entrada y salida, con validaciones DataAnnotations |
| `Interfaces/IServicios.cs` | interfaces | `ISaludoService.Saludar(nombre)`, `ICalculadoraService.Calcular(req)`, `ICatalogoService.ObtenerLenguajesAsync(ct)` | Contratos de negocio; los controladores dependen de ellas |
| `Services/SaludoService.cs` | clase | `Saludar` | Arma el mensaje; recibe `TimeProvider` para poder fijar la hora en pruebas |
| `Services/CalculadoraService.cs` | clase | `Calcular` | `switch` de operaciones; lanza `ReglaNegocioException` en división entre 0 u operación desconocida |
| `Services/CatalogoService.cs` | clase | `ObtenerLenguajesAsync` | Devuelve las opciones del combo; `async` + `CancellationToken` simulando BD |
| `Controllers/SaludoController.cs` | controlador | `Get()`, `Post(SaludoRequest)` | `GET/POST /api/saludo` |
| `Controllers/CalculadoraController.cs` | controlador | `Calcular(CalculoRequest)` | `POST /api/calculadora` |
| `Controllers/CatalogosController.cs` | controlador | `Lenguajes(ct)` | `GET /api/catalogos/lenguajes` |
| `Exceptions/ReglaNegocioException.cs` | excepción | — | Error de negocio esperado → HTTP 400 |
| `Middleware/ManejadorErroresGlobal.cs` | `IExceptionHandler` | `TryHandleAsync` | Convierte cualquier excepción en ProblemDetails (400, 499 o 500 sin exponer detalle) |
| `appsettings.json` | config | `Cors:Origenes` | Orígenes permitidos (4200 Angular, 5173 React) |

### Pruebas (`backend/HolaMundo.Tests`)

| Archivo | Tipo de prueba | Qué valida |
|---|---|---|
| `CalculadoraServiceTests.cs` | unitaria (`[Theory]`, `[Fact]`) | las 4 operaciones y la división entre cero |
| `ApiIntegracionTests.cs` | integración (`WebApplicationFactory<Program>`) | la API completa en memoria: saludo, 400 por validación, ProblemDetails, combo |

### Frontend (`holamundo-web/src`)

| Archivo | Tipo | Miembros | Responsabilidad |
|---|---|---|---|
| `environments/environment.ts` | constante | `apiUrl` | URL base de la API |
| `app/app.config.ts` | config | `provideHttpClient()` | Equivale a `Program.cs`: registra HttpClient |
| `app/models/contratos.ts` | interfaces TS | espejo de los DTO + `ProblemDetails` | Tipado del JSON |
| `app/services/hola-mundo-api.service.ts` | `@Injectable` (singleton) | `saludar`, `calcular`, `lenguajes`, `aMensaje` | Único punto HTTP; traduce errores a texto |
| `app/componentes/saludo/saludo.component.ts` | componente standalone | `saludar()`; signals `mensaje`, `error`, `cargando` | Input de texto |
| `app/componentes/calculadora/calculadora.component.ts` | componente standalone | `calcular()`; `operaciones` | Calculadora |
| `app/componentes/combo-lenguajes/combo-lenguajes.component.ts` | componente standalone | `ngOnInit()`; signals `opciones`, `seleccion`; `computed` `mensaje` | Combobox con mensaje |
| `app/app.component.*` | componente raíz | — | Compone las tres tarjetas |
| `app/app.component.spec.ts` | prueba Jasmine | `HttpTestingController` | Prueba el servicio sin backend |

## 6. Conceptos clave

| Concepto | Dónde verlo | En una frase |
|---|---|---|
| Inyección de dependencias | `Program.cs` (`AddScoped`), constructores primarios de los controladores, `inject()` en Angular | El framework crea los objetos y los entrega; tú solo declaras qué necesitas |
| Ciclos de vida DI | `Program.cs` | Singleton (uno para toda la app), Scoped (uno por petición), Transient (uno nuevo cada vez) |
| async/await + cancelación | `CatalogoService`, `CatalogosController` | Si el cliente cierra la petición, el `CancellationToken` detiene el trabajo |
| Validación | DataAnnotations en `Contratos.cs` + `[ApiController]` | 400 automático antes de entrar al controlador |
| Manejo consistente de errores | `ManejadorErroresGlobal` | Todos los errores salen en ProblemDetails (RFC 7807) |
| CORS | `Program.cs` + `appsettings.json` | El navegador bloquea otro origen salvo que el servidor lo permita |
| Observables (RxJS) | `HolaMundoApiService` | `HttpClient` devuelve un flujo; nada se envía hasta que alguien hace `subscribe` |
| Signals | componentes | Estado reactivo de Angular 17; `computed` deriva valores |
| Control flow nuevo | plantillas (`@if`, `@for`) | Reemplaza `*ngIf` y `*ngFor` desde Angular 17 |
| Pruebas | `HolaMundo.Tests`, `app.component.spec.ts` | Unitarias (servicio solo) vs integración (API en memoria) |

## 7. Plantilla para una HU nueva (ejemplo: "como usuario quiero despedirme")

1. **Contrato**: agrega `DespedidaResponse` en `Contracts/Contratos.cs`.
2. **Interfaz**: agrega `Despedir(string nombre)` en `ISaludoService`.
3. **Servicio**: impleméntalo en `SaludoService`.
4. **Controlador**: `[HttpPost("despedida")]` en `SaludoController` → `POST /api/saludo/despedida`.
5. **Prueba .NET**: un `[Fact]` en `ApiIntegracionTests`.
6. **Modelo TS**: `DespedidaResponse` en `models/contratos.ts`.
7. **Servicio Angular**: método `despedir()` en `HolaMundoApiService`.
8. **Componente**: botón nuevo en `SaludoComponent` o un componente `app-despedida`.
9. **Prueba Angular**: un `it(...)` con `HttpTestingController`.
10. **Documento de transición** de la HU con esta misma estructura (secciones 3, 4 y 5).

## 8. Problemas comunes

| Síntoma | Causa | Solución |
|---|---|---|
| "No hay conexión con la API" en Angular | el backend no está corriendo o usa otro puerto | `dotnet run --launch-profile http` y revisa que diga 5080 |
| Error CORS en la consola del navegador | el origen no está en `Cors:Origenes` | agrega el origen en `appsettings.json` |
| 500 "validation metadata ... will be ignored" | atributos con `[property: Required]` en un record | en records de MVC van en el parámetro: `[Required] string Nombre` (fue un error real al crear este proyecto) |
| JSON llega con `null` en Angular | usaste PascalCase en TS | .NET serializa en camelCase: `mensaje`, no `Mensaje` |
