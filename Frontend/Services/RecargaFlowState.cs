namespace BancoArboleda.Web.Services;

public class RecargaFlowState
{
    // Datos ingresados en el Paso 1
    public string NumeroCelular { get; set; } = string.Empty;
    public int? OperadorId { get; set; }
    public string OperadorNombre { get; set; } = string.Empty;
    public decimal MontoMin { get; set; } = 5;
    public decimal MontoMax { get; set; } = 100;

    // Datos ingresados en el Paso 2
    public decimal? Monto { get; set; }

    // Datos resultantes para la Confirmación
    public string ConfirmacionId { get; set; } = string.Empty;
    public string ConfirmacionFecha { get; set; } = string.Empty;

    /// <summary>
    /// Limpia el estado de la recarga una vez finalizada la confirmación o si se desea reiniciar.
    /// </summary>
    public void Reset()
    {
        NumeroCelular = string.Empty;
        OperadorId = null;
        OperadorNombre = string.Empty;
        MontoMin = 5;
        MontoMax = 100;
        Monto = null;
        ConfirmacionId = string.Empty;
        ConfirmacionFecha = string.Empty;
    }

    /// <summary>
    /// Valida si el Paso 1 tiene datos correctos para pasar al Paso 2.
    /// </summary>
    public bool TieneDatosPaso1()
    {
        return !string.IsNullOrEmpty(NumeroCelular) && OperadorId.HasValue && !string.IsNullOrEmpty(OperadorNombre);
    }

    /// <summary>
    /// Valida si el Paso 2 tiene datos correctos para pasar al Paso 3.
    /// </summary>
    public bool TieneDatosPaso2()
    {
        return TieneDatosPaso1() && Monto.HasValue && Monto >= MontoMin && Monto <= MontoMax;
    }

    /// <summary>
    /// Valida si hay datos de confirmación para mostrar el recibo.
    /// </summary>
    public bool TieneConfirmacion()
    {
        return !string.IsNullOrEmpty(ConfirmacionId) && !string.IsNullOrEmpty(ConfirmacionFecha);
    }
}
