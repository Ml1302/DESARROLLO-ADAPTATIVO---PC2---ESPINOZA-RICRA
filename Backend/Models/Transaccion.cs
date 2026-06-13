namespace BancoArboleda.Backend.Models;

public class Transaccion
{
    public string Id { get; set; } = string.Empty;
    public string NumeroCelular { get; set; } = string.Empty;
    public int OperadorId { get; set; }
    public string OperadorNombre { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string Fecha { get; set; } = string.Empty;
    public string Estado { get; set; } = "simulado";
}
