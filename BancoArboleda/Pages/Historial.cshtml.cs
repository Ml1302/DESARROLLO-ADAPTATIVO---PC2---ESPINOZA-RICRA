using Microsoft.AspNetCore.Mvc.RazorPages;
using Dapper;
using BancoArboleda.Data;
using BancoArboleda.Models;

namespace BancoArboleda.Pages;

public class HistorialModel : PageModel
{
    private readonly DatabaseContext _db;

    public HistorialModel(DatabaseContext db) => _db = db;

    public IReadOnlyList<Transaccion> Transacciones { get; private set; } = [];

    public async Task OnGetAsync()
    {
        using var conn = _db.CreateConnection();
        conn.Open();
        var result = await conn.QueryAsync<Transaccion>(
            @"SELECT
                id              as Id,
                numero_celular  as NumeroCelular,
                operador_id     as OperadorId,
                operador_nombre as OperadorNombre,
                monto           as Monto,
                fecha           as Fecha,
                estado          as Estado
              FROM transacciones
              ORDER BY fecha DESC
              LIMIT 10");

        Transacciones = result.ToList();
    }
}
