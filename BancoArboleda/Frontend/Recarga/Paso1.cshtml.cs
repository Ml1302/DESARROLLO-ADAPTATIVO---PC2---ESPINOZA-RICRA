using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using BancoArboleda.Backend.Models;
using BancoArboleda.Backend.Services;

namespace BancoArboleda.Frontend.Recarga;

public class Paso1Model : PageModel
{
    private readonly OperadorService _operadorService;

    public Paso1Model(OperadorService operadorService)
    {
        _operadorService = operadorService;
    }

    [BindProperty]
    [Required(ErrorMessage = "El número de celular es obligatorio.")]
    [RegularExpression(@"^9\d{8}$", ErrorMessage = "El número debe tener exactamente 9 dígitos y comenzar con 9.")]
    public string NumeroCelular { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Debe seleccionar un operador.")]
    public int? OperadorId { get; set; }

    public List<SelectListItem> Operadores { get; set; } = new();

    private async Task CargarOperadores()
    {
        var ops = await _operadorService.GetAllAsync();
        Operadores = ops.Select(o => new SelectListItem
        {
            Value = o.Id.ToString(),
            Text  = o.Nombre
        }).ToList();
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await CargarOperadores();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await CargarOperadores();

        if (!ModelState.IsValid)
            return Page();

        var op = await _operadorService.GetByIdAsync(OperadorId!.Value);

        TempData["Paso1_Numero"]          = NumeroCelular;
        TempData["Paso1_OperadorId"]      = OperadorId!.Value;
        TempData["Paso1_OperadorNombre"]  = op?.Nombre ?? "";
        TempData["Paso1_MontoMin"]        = op?.MontoMin.ToString("F2") ?? "5";
        TempData["Paso1_MontoMax"]        = op?.MontoMax.ToString("F2") ?? "100";

        return RedirectToPage("Paso2");
    }
}
