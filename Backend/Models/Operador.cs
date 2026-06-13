namespace BancoArboleda.Backend.Models;

public class Operador
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal MontoMin { get; set; }
    public decimal MontoMax { get; set; }
}
