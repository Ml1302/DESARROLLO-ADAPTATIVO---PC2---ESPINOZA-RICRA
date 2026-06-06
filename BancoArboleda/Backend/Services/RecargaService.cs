using Dapper;
using BancoArboleda.Backend.Data;
using BancoArboleda.Backend.Models;

namespace BancoArboleda.Backend.Services;

/// <summary>
/// Servicio de acceso a datos para recargas y transacciones.
/// Centraliza todas las consultas SQL relacionadas con la tabla transacciones.
/// </summary>
public class RecargaService
{
    private readonly DatabaseContext _db;

    public RecargaService(DatabaseContext db) => _db = db;

    /// <summary>
    /// Inserta una nueva transacción en la base de datos.
    /// El ID (GUID) debe venir ya generado en el objeto Transaccion.
    /// </summary>
    public async Task InsertarAsync(Transaccion transaccion)
    {
        using var conn = _db.CreateConnection();
        conn.Open();
        await conn.ExecuteAsync(
            @"INSERT INTO transacciones (id, numero_celular, operador_id, operador_nombre, monto, fecha, estado)
              VALUES (@Id, @NumeroCelular, @OperadorId, @OperadorNombre, @Monto, @Fecha, @Estado)",
            transaccion);
    }

    /// <summary>
    /// Devuelve las últimas 10 transacciones ordenadas por fecha descendente.
    /// </summary>
    public async Task<IReadOnlyList<Transaccion>> GetHistorialAsync()
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
        return result.ToList();
    }
}
