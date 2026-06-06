using BancoArboleda.Backend.Data;
using BancoArboleda.Backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Cadena de conexión SQLite
builder.Configuration["ConnectionStrings:DefaultConnection"] = "Data Source=banco.db";

// ── Servicios ──────────────────────────────────────────────
builder.Services.AddRazorPages(options =>
{
    // Apunta a la carpeta Frontend/ como raíz de las Razor Pages
    options.RootDirectory = "/Frontend";
});

// Datos (Backend)
builder.Services.AddSingleton<DatabaseContext>();
builder.Services.AddSingleton<DbSeeder>();

// Servicios de negocio (Backend)
builder.Services.AddScoped<OperadorService>();
builder.Services.AddScoped<RecargaService>();

// TempData con Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// ── Inicialización de BD y Seed ────────────────────────────
var dbContext = app.Services.GetRequiredService<DatabaseContext>();
dbContext.Initialize();

var seeder = app.Services.GetRequiredService<DbSeeder>();
seeder.Seed();

// ── Pipeline ───────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
