using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Dapper;
using BancoArboleda.Data;
using BancoArboleda.Models;

namespace BancoArboleda.Pages.Recarga;

public class Paso1Model : PageModel
{
    private readonly DatabaseContext _db;

    public Paso1Model(DatabaseContext db) => _db = db;

    [BindProperty]
    [Required(ErrorMessage = "El número de celular es obligatorio.")]
    [RegularExpression(@"^9\d{8}$", ErrorMessage = "El número debe tener exactamente 9 dígitos y comenzar con 9.")]
    public string NumeroCelular { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Debe seleccionar un operador.")]
    public int OperadorId { get; set; }

    public List<SelectListItem> Operadores { get; set; } = new();

    private async Task CargarOperadores()
    {
        using var conn = _db.CreateConnection();
        conn.Open();
        var ops = await conn.QueryAsync<Operador>(
            "SELECT id as Id, nombre as Nombre, monto_min as MontoMin, monto_max as MontoMax FROM operadores");
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

        TempData["Paso1_Numero"]     = NumeroCelular;
        TempData["Paso1_OperadorId"] = OperadorId;

        // Obtener nombre del operador para mostrar en pasos siguientes
        using var conn = _db.CreateConnection();
        conn.Open();
        var op = await conn.QueryFirstOrDefaultAsync<Operador>(
            "SELECT id as Id, nombre as Nombre, monto_min as MontoMin, monto_max as MontoMax FROM operadores WHERE id = @id",
            new { id = OperadorId });

        TempData["Paso1_OperadorNombre"] = op?.Nombre ?? "";
        TempData["Paso1_MontoMin"]       = op?.MontoMin.ToString("F2") ?? "5";
        TempData["Paso1_MontoMax"]       = op?.MontoMax.ToString("F2") ?? "100";

        return RedirectToPage("Paso2");
    }
}
