using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BancoArboleda.Pages.Recarga;

public class ConfirmacionModel : PageModel
{
    public string TransaccionId  { get; private set; } = string.Empty;
    public string NumeroCelular  { get; private set; } = string.Empty;
    public string OperadorNombre { get; private set; } = string.Empty;
    public string Monto          { get; private set; } = string.Empty;
    public string Fecha          { get; private set; } = string.Empty;

    public IActionResult OnGet()
    {
        if (TempData["Confirm_Id"] is not string id || string.IsNullOrEmpty(id))
            return RedirectToPage("/Index");

        TransaccionId  = id;
        NumeroCelular  = TempData["Confirm_Numero"]  as string ?? "";
        OperadorNombre = TempData["Confirm_Operador"] as string ?? "";
        Monto          = TempData["Confirm_Monto"]   as string ?? "";
        Fecha          = TempData["Confirm_Fecha"]   as string ?? "";

        return Page();
    }
}
