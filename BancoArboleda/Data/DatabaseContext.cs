using Microsoft.Data.Sqlite;

namespace BancoArboleda.Data;

public class DatabaseContext
{
    private readonly string _connectionString;

    public DatabaseContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=banco.db";
    }

    public SqliteConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }

    public void Initialize()
    {
        using var connection = CreateConnection();
        connection.Open();

        var createOperadores = """
            CREATE TABLE IF NOT EXISTS operadores (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                nombre TEXT NOT NULL,
                monto_min REAL NOT NULL,
                monto_max REAL NOT NULL
            )
            """;

        var createTransacciones = """
            CREATE TABLE IF NOT EXISTS transacciones (
                id TEXT PRIMARY KEY,
                numero_celular TEXT NOT NULL,
                operador_id INTEGER NOT NULL,
                operador_nombre TEXT NOT NULL,
                monto REAL NOT NULL,
                fecha TEXT NOT NULL,
                estado TEXT NOT NULL DEFAULT 'simulado'
            )
            """;

        using var cmd1 = new SqliteCommand(createOperadores, connection);
        cmd1.ExecuteNonQuery();

        using var cmd2 = new SqliteCommand(createTransacciones, connection);
        cmd2.ExecuteNonQuery();
    }
}
