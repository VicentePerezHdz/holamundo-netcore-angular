using HolaMundo.Api.Interfaces;
using HolaMundo.Api.Middleware;
using HolaMundo.Api.Services;

// ===== 1. Registro de servicios (contenedor de inyección de dependencias) =====
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();               // documentación interactiva en /swagger
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ManejadorErroresGlobal>();

// Tiempos de vida: Singleton = una instancia para toda la app; Scoped = una por petición HTTP;
// Transient = una nueva cada vez que se pide.
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<ISaludoService, SaludoService>();
builder.Services.AddScoped<ICalculadoraService, CalculadoraService>();
builder.Services.AddScoped<ICatalogoService, CatalogoService>();

// CORS: el navegador bloquea llamadas de http://localhost:4200 (Angular) a http://localhost:5080 (.NET)
// porque son "orígenes" distintos. Esta política le dice al navegador que sí están permitidas.
const string PoliticaFront = "front-local";
builder.Services.AddCors(o => o.AddPolicy(PoliticaFront, p => p
    .WithOrigins(builder.Configuration.GetSection("Cors:Origenes").Get<string[]>() ?? [])
    .AllowAnyHeader()
    .AllowAnyMethod()));

// ===== 2. Pipeline HTTP (middlewares; el orden importa) =====
var app = builder.Build();

app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(PoliticaFront);
app.MapControllers();

app.Run();

// Permite que las pruebas de integración (WebApplicationFactory<Program>) encuentren esta clase.
public partial class Program;
