using Dapper;
using BancoArboleda.Models;

namespace BancoArboleda.Data;

public class DbSeeder
{
    private readonly DatabaseContext _context;

    public DbSeeder(DatabaseContext context)
    {
        _context = context;
    }

    public void Seed()
    {
        using var connection = _context.CreateConnection();
        connection.Open();

        // Seed operadores si la tabla está vacía
        var count = connection.QuerySingle<int>("SELECT COUNT(*) FROM operadores");
        if (count == 0)
        {
            var operadores = new[]
            {
                new { nombre = "Movistar", monto_min = 5.0, monto_max = 100.0 },
                new { nombre = "Claro",    monto_min = 5.0, monto_max = 100.0 },
                new { nombre = "Entel",    monto_min = 5.0, monto_max = 50.0  }
            };

            foreach (var op in operadores)
            {
                connection.Execute(
                    "INSERT INTO operadores (nombre, monto_min, monto_max) VALUES (@nombre, @monto_min, @monto_max)",
                    op);
            }
        }

        // Seed transacciones de ejemplo si la tabla está vacía
        var txCount = connection.QuerySingle<int>("SELECT COUNT(*) FROM transacciones");
        if (txCount == 0)
        {
            var operadores = connection.Query<Operador>(
                "SELECT id as Id, nombre as Nombre, monto_min as MontoMin, monto_max as MontoMax FROM operadores"
            ).ToList();

            var ejemplos = new[]
            {
                new { celular = "987654321", opIdx = 0, monto = 20.0m,  dias = 5  },
                new { celular = "912345678", opIdx = 1, monto = 50.0m,  dias = 10 },
                new { celular = "956781234", opIdx = 2, monto = 10.0m,  dias = 15 },
                new { celular = "943210987", opIdx = 0, monto = 100.0m, dias = 20 },
                new { celular = "998877665", opIdx = 1, monto = 5.0m,   dias = 25 }
            };

            foreach (var ej in ejemplos)
            {
                var op = operadores[ej.opIdx];
                var fecha = DateTime.Now.AddDays(-ej.dias).ToString("yyyy-MM-dd HH:mm:ss");
                connection.Execute(
                    @"INSERT INTO transacciones (id, numero_celular, operador_id, operador_nombre, monto, fecha, estado)
                      VALUES (@id, @numero_celular, @operador_id, @operador_nombre, @monto, @fecha, @estado)",
                    new
                    {
                        id             = Guid.NewGuid().ToString(),
                        numero_celular = ej.celular,
                        operador_id    = op.Id,
                        operador_nombre = op.Nombre,
                        monto          = ej.monto,
                        fecha          = fecha,
                        estado         = "simulado"
                    });
            }
        }
    }
}
