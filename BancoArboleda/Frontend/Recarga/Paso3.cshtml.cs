using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using BancoArboleda.Backend.Models;
using BancoArboleda.Backend.Services;

namespace BancoArboleda.Frontend.Recarga;

public class Paso3Model : PageModel
{
    private readonly RecargaService _recargaService;

    public Paso3Model(RecargaService recargaService)
    {
        _recargaService = recargaService;
    }

    public string NumeroCelular  { get; private set; } = string.Empty;
    public string OperadorNombre { get; private set; } = string.Empty;
    public string Monto          { get; private set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "La clave es obligatoria.")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "La clave debe tener exactamente 6 dígitos numéricos.")]
    public string ClaveOnline { get; set; } = string.Empty;

    private bool CargarDatosPrevios()
    {
        if (TempData["Paso1_Numero"] is not string numero || string.IsNullOrEmpty(numero))
            return false;

        TempData.Keep("Paso1_Numero");
        TempData.Keep("Paso1_OperadorId");
        TempData.Keep("Paso1_OperadorNombre");
        TempData.Keep("Paso2_Monto");

        NumeroCelular  = numero;
        OperadorNombre = TempData["Paso1_OperadorNombre"] as string ?? "";
        Monto          = TempData["Paso2_Monto"] as string ?? "";
        return true;
    }

    public IActionResult OnGet()
    {
        if (!CargarDatosPrevios())
            return RedirectToPage("Paso1");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!CargarDatosPrevios())
            return RedirectToPage("Paso1");

        if (!ModelState.IsValid)
            return Page();

        if (ClaveOnline != "123456")
        {
            ModelState.AddModelError("ClaveOnline", "La clave online ingresada es incorrecta.");
            return Page();
        }

        var numero     = TempData["Paso1_Numero"]       as string ?? "";
        var operadorId = TempData["Paso1_OperadorId"] is int oid ? oid : 0;
        var opNombre   = TempData["Paso1_OperadorNombre"] as string ?? "";
        var montoStr   = TempData["Paso2_Monto"]        as string ?? "0";

        decimal.TryParse(montoStr,
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out var monto);

        var transaccion = new Transaccion
        {
            Id              = Guid.NewGuid().ToString(),
            NumeroCelular   = numero,
            OperadorId      = operadorId,
            OperadorNombre  = opNombre,
            Monto           = monto,
            Fecha           = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Estado          = "simulado"
        };

        // Delegar el INSERT al RecargaService (Backend)
        await _recargaService.InsertarAsync(transaccion);

        TempData.Clear();
        TempData["Confirm_Id"]       = transaccion.Id;
        TempData["Confirm_Numero"]   = transaccion.NumeroCelular;
        TempData["Confirm_Operador"] = transaccion.OperadorNombre;
        TempData["Confirm_Monto"]    = transaccion.Monto.ToString("F2");
        TempData["Confirm_Fecha"]    = transaccion.Fecha;

        return RedirectToPage("Confirmacion");
    }
}
