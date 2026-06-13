using Dapper;
using BancoArboleda.Backend.Data;
using BancoArboleda.Backend.Models;

namespace BancoArboleda.Backend.Services;

/// <summary>
/// Servicio de acceso a datos para operadores móviles.
/// Centraliza todas las consultas SQL relacionadas con la tabla operadores.
/// </summary>
public class OperadorService
{
    private readonly DatabaseContext _db;

    public OperadorService(DatabaseContext db) => _db = db;

    /// <summary>Devuelve todos los operadores disponibles.</summary>
    public async Task<IEnumerable<Operador>> GetAllAsync()
    {
        using var conn = _db.CreateConnection();
        conn.Open();
        return await conn.QueryAsync<Operador>(
            "SELECT id as Id, nombre as Nombre, monto_min as MontoMin, monto_max as MontoMax FROM operadores");
    }

    /// <summary>Devuelve un operador por su ID, o null si no existe.</summary>
    public async Task<Operador?> GetByIdAsync(int id)
    {
        using var conn = _db.CreateConnection();
        conn.Open();
        return await conn.QueryFirstOrDefaultAsync<Operador>(
            "SELECT id as Id, nombre as Nombre, monto_min as MontoMin, monto_max as MontoMax FROM operadores WHERE id = @id",
            new { id });
    }
}
