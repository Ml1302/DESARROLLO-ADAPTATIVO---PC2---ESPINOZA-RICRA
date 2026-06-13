using BancoArboleda.Backend.Data;
using BancoArboleda.Backend.Services;
using BancoArboleda.Backend.Models;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cadena de conexión SQLite
builder.Configuration["ConnectionStrings:DefaultConnection"] = "Data Source=banco.db";

// Registro de servicios de persistencia y negocio (Backend)
builder.Services.AddSingleton<DatabaseContext>();
builder.Services.AddSingleton<DbSeeder>();
builder.Services.AddScoped<OperadorService>();
builder.Services.AddScoped<RecargaService>();

// Configuración de CORS para permitir peticiones desde el cliente Blazor WebAssembly
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configuración del pipeline de desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

// ── Inicialización de BD y Seed ────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    dbContext.Initialize();

    var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
    seeder.Seed();
}

// ── Endpoints Minimal API (Backend) ─────────────────────────

// GET /api/operadores -> Obtener todos los operadores
app.MapGet("/api/operadores", async (OperadorService service) =>
{
    var operadores = await service.GetAllAsync();
    return Results.Ok(operadores);
})
.WithName("GetAllOperadores")
.WithOpenApi();

// GET /api/operadores/{id} -> Obtener operador por ID
app.MapGet("/api/operadores/{id:int}", async (int id, OperadorService service) =>
{
    var operador = await service.GetByIdAsync(id);
    return operador is not null ? Results.Ok(operador) : Results.NotFound();
})
.WithName("GetOperadorById")
.WithOpenApi();

// GET /api/transacciones/historial -> Obtener últimas 10 transacciones
app.MapGet("/api/transacciones/historial", async (RecargaService service) =>
{
    var historial = await service.GetHistorialAsync();
    return Results.Ok(historial);
})
.WithName("GetHistorialTransacciones")
.WithOpenApi();

// POST /api/transacciones -> Registrar nueva recarga
app.MapPost("/api/transacciones", async (Transaccion transaccion, RecargaService service, OperadorService operadorService) =>
{
    if (transaccion == null)
    {
        return Results.BadRequest("La transacción no puede ser nula.");
    }

    // Validar Número de Celular (debe empezar con 9, tener exactamente 9 dígitos y ser solo números)
    if (string.IsNullOrEmpty(transaccion.NumeroCelular) || 
        transaccion.NumeroCelular.Length != 9 || 
        !transaccion.NumeroCelular.StartsWith('9') || 
        !transaccion.NumeroCelular.All(char.IsDigit))
    {
        return Results.BadRequest("El número de celular debe tener exactamente 9 dígitos y comenzar con 9.");
    }

    // Validar que el operador exista en el sistema
    var op = await operadorService.GetByIdAsync(transaccion.OperadorId);
    if (op == null)
    {
        return Results.BadRequest("El operador seleccionado no es válido.");
    }

    // Validar que el nombre del operador coincida con el registrado
    if (transaccion.OperadorNombre != op.Nombre)
    {
        return Results.BadRequest("El nombre del operador no coincide con el ID.");
    }

    // Validar Límites de Monto
    if (transaccion.Monto < op.MontoMin || transaccion.Monto > op.MontoMax)
    {
        return Results.BadRequest($"El monto debe estar entre S/{op.MontoMin:F2} y S/{op.MontoMax:F2} para {op.Nombre}.");
    }

    // Validar Máximo un decimal (ej. 10.50)
    if ((transaccion.Monto * 10) % 1 != 0)
    {
        return Results.BadRequest("El monto solo puede tener como máximo un decimal (ej: 10.50).");
    }

    // Validar Fecha
    if (string.IsNullOrEmpty(transaccion.Fecha))
    {
        return Results.BadRequest("La fecha de la transacción es obligatoria.");
    }
    
    await service.InsertarAsync(transaccion);
    return Results.Created($"/api/transacciones/{transaccion.Id}", transaccion);
})
.WithName("RegistrarTransaccion")
.WithOpenApi();

app.Run();

