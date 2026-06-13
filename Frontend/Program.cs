using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BancoArboleda.Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Registrar el contenedor de estado del flujo de recargas
builder.Services.AddScoped<BancoArboleda.Web.Services.RecargaFlowState>();

// Registrar los servicios clientes con su respectivo HttpClient apuntando al Backend
var backendApiUrl = "http://localhost:5000/";
builder.Services.AddHttpClient<BancoArboleda.Backend.Services.OperadorService>(client =>
{
    client.BaseAddress = new Uri(backendApiUrl);
});
builder.Services.AddHttpClient<BancoArboleda.Backend.Services.RecargaService>(client =>
{
    client.BaseAddress = new Uri(backendApiUrl);
});

await builder.Build().RunAsync();
