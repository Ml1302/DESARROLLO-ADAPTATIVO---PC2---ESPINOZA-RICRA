using Microsoft.AspNetCore.Mvc.RazorPages;
using BancoArboleda.Backend.Models;
using BancoArboleda.Backend.Services;

namespace BancoArboleda.Frontend;

public class HistorialModel : PageModel
{
    private readonly RecargaService _recargaService;

    public HistorialModel(RecargaService recargaService)
    {
        _recargaService = recargaService;
    }

    public IReadOnlyList<Transaccion> Transacciones { get; private set; } = [];

    public async Task OnGetAsync()
    {
        Transacciones = await _recargaService.GetHistorialAsync();
    }
}
