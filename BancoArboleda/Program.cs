using BancoArboleda.Data;

var builder = WebApplication.CreateBuilder(args);

// Configurar cadena de conexión SQLite
builder.Configuration["ConnectionStrings:DefaultConnection"] = "Data Source=banco.db";

// Servicios
builder.Services.AddRazorPages();
builder.Services.AddSingleton<DatabaseContext>();
builder.Services.AddSingleton<DbSeeder>();

// TempData con Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Inicializar base de datos y seed
var dbContext = app.Services.GetRequiredService<DatabaseContext>();
dbContext.Initialize();

var seeder = app.Services.GetRequiredService<DbSeeder>();
seeder.Seed();

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
