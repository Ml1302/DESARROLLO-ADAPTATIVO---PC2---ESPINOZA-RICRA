using BancoArboleda.Backend.Models;
using System.Net.Http.Json;

namespace BancoArboleda.Backend.Services;

public class OperadorService
{
    private readonly HttpClient _httpClient;

    public OperadorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Operador>> GetAllAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<IEnumerable<Operador>>("api/operadores");
        return response ?? Enumerable.Empty<Operador>();
    }

    public async Task<Operador?> GetByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Operador>($"api/operadores/{id}");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
