namespace ITManager.Models;

public class Repuesto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int StockActual { get; set; } = 0;
    public int StockMinimo { get; set; } = 0;
    public decimal CostoUnitario { get; set; } = 0m;

    // Propiedades de navegación
    public virtual ICollection<DetalleMantenimiento> Detalles { get; set; } = new List<DetalleMantenimiento>();

    public Repuesto()
    {
    }

    public Repuesto(int id, string codigo, string nombre, int stock, int min, decimal costo)
    {
        Id = id;
        Codigo = codigo;
        Nombre = nombre;
        StockActual = stock;
        StockMinimo = min;
        CostoUnitario = costo;
    }

    public bool DescontarStock(int cantidad)
    {
        if (cantidad <= 0 || StockActual < cantidad)
        {
            return false;
        }

        StockActual -= cantidad;
        return true;
    }

    public void ReponerStock(int cantidad)
    {
        if (cantidad > 0)
        {
            StockActual += cantidad;
        }
    }

    public bool RequiereReposicion()
    {
        return StockActual <= StockMinimo;
    }
}

