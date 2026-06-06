using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Dapper;
using BancoArboleda.Data;

namespace BancoArboleda.Pages.Recarga;

public class Paso3Model : PageModel
{
    private readonly DatabaseContext _db;

    public Paso3Model(DatabaseContext db) => _db = db;

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

        // Obtener datos completos del TempData
        var numero      = TempData["Paso1_Numero"] as string ?? "";
        var operadorId  = TempData["Paso1_OperadorId"] is int oid ? oid : 0;
        var opNombre    = TempData["Paso1_OperadorNombre"] as string ?? "";
        var montoStr    = TempData["Paso2_Monto"] as string ?? "0";
        decimal.TryParse(montoStr, System.Globalization.NumberStyles.Any,
                         System.Globalization.CultureInfo.InvariantCulture, out var monto);

        var id    = Guid.NewGuid().ToString();
        var fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        using var conn = _db.CreateConnection();
        conn.Open();
        await conn.ExecuteAsync(
            @"INSERT INTO transacciones (id, numero_celular, operador_id, operador_nombre, monto, fecha, estado)
              VALUES (@id, @numero_celular, @operador_id, @operador_nombre, @monto, @fecha, @estado)",
            new
            {
                id,
                numero_celular  = numero,
                operador_id     = operadorId,
                operador_nombre = opNombre,
                monto,
                fecha,
                estado          = "simulado"
            });

        TempData.Clear();
        TempData["Confirm_Id"]      = id;
        TempData["Confirm_Numero"]  = numero;
        TempData["Confirm_Operador"] = opNombre;
        TempData["Confirm_Monto"]   = monto.ToString("F2");
        TempData["Confirm_Fecha"]   = fecha;

        return RedirectToPage("Confirmacion");
    }
}
