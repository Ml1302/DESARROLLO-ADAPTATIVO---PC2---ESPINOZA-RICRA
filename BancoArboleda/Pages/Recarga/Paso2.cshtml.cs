using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BancoArboleda.Pages.Recarga;

public class Paso2Model : PageModel
{
    // Datos de sesión del paso anterior
    public string NumeroCelular { get; private set; } = string.Empty;
    public int    OperadorId    { get; private set; }
    public string OperadorNombre { get; private set; } = string.Empty;
    public decimal MontoMin { get; private set; } = 5;
    public decimal MontoMax { get; private set; } = 100;

    [BindProperty]
    [Required(ErrorMessage = "Debes ingresar un monto.")]
    public decimal? Monto { get; set; }

    public string? ErrorMonto { get; private set; }

    private bool CargarDatosPrevios()
    {
        if (TempData["Paso1_Numero"] is not string numero || string.IsNullOrEmpty(numero))
            return false;

        TempData.Keep("Paso1_Numero");
        TempData.Keep("Paso1_OperadorId");
        TempData.Keep("Paso1_OperadorNombre");
        TempData.Keep("Paso1_MontoMin");
        TempData.Keep("Paso1_MontoMax");

        NumeroCelular  = numero;
        OperadorId     = TempData["Paso1_OperadorId"] is int oid ? oid : 0;
        OperadorNombre = TempData["Paso1_OperadorNombre"] as string ?? "";

        if (decimal.TryParse(TempData["Paso1_MontoMin"]?.ToString(), out var min)) MontoMin = min;
        if (decimal.TryParse(TempData["Paso1_MontoMax"]?.ToString(), out var max)) MontoMax = max;

        return true;
    }

    public IActionResult OnGet()
    {
        if (!CargarDatosPrevios())
            return RedirectToPage("Paso1");

        return Page();
    }

    public IActionResult OnPost()
    {
        if (!CargarDatosPrevios())
            return RedirectToPage("Paso1");

        if (!ModelState.IsValid || Monto is null)
        {
            ErrorMonto = "Debes ingresar un monto válido.";
            return Page();
        }

        if (Monto < MontoMin || Monto > MontoMax)
        {
            ErrorMonto = $"El monto debe estar entre S/{MontoMin:F2} y S/{MontoMax:F2} para {OperadorNombre}.";
            ModelState.AddModelError(nameof(Monto), ErrorMonto);
            return Page();
        }

        // Conservar TempData del paso 1 y agregar monto
        TempData["Paso1_Numero"]       = NumeroCelular;
        TempData["Paso1_OperadorId"]   = OperadorId;
        TempData["Paso1_OperadorNombre"] = OperadorNombre;
        TempData["Paso1_MontoMin"]     = MontoMin.ToString("F2");
        TempData["Paso1_MontoMax"]     = MontoMax.ToString("F2");
        TempData["Paso2_Monto"]        = Monto.Value.ToString("F2");

        return RedirectToPage("Paso3");
    }
}
