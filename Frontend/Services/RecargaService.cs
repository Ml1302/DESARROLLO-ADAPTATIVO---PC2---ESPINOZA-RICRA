using BancoArboleda.Backend.Models;
using System.Net.Http.Json;

namespace BancoArboleda.Backend.Services;

public class RecargaService
{
    private readonly HttpClient _httpClient;

    public RecargaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task InsertarAsync(Transaccion transaccion)
    {
        var response = await _httpClient.PostAsJsonAsync("api/transacciones", transaccion);
        response.EnsureSuccessStatusCode();
    }

    public async Task<IReadOnlyList<Transaccion>> GetHistorialAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<List<Transaccion>>("api/transacciones/historial");
        return response ?? new List<Transaccion>();
    }
}
