namespace ITManager.Models;

public class DetalleMantenimiento
{
    public int Id { get; set; }
    public int MantenimientoId { get; set; }
    public int RepuestoId { get; set; }
    public int Cantidad { get; set; } = 1;
    public decimal CostoHistorico { get; set; } = 0m;

    // Propiedades de navegación
    public virtual Mantenimiento? Mantenimiento { get; set; }
    public virtual Repuesto? Repuesto { get; set; }

    public DetalleMantenimiento()
    {
    }

    public DetalleMantenimiento(int mantenimientoId, int repuestoId, int cantidad, decimal costoHistorico)
    {
        MantenimientoId = mantenimientoId;
        RepuestoId = repuestoId;
        Cantidad = cantidad;
        CostoHistorico = costoHistorico;
    }

    public decimal GetSubtotal()
    {
        return Cantidad * CostoHistorico;
    }
}

