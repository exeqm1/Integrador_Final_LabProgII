using ITManager.Models.Enums;

namespace ITManager.Models;

public class Mantenimiento
{
    public int Id { get; set; }
    public int EquipoId { get; set; }
    public int? TicketId { get; set; }
    public int TecnicoId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public TipoMantenimiento Tipo { get; set; } = TipoMantenimiento.CORRECTIVO;
    public string Diagnostico { get; set; } = string.Empty;
    public string TareasRealizadas { get; set; } = string.Empty;
    public decimal HorasTrabajo { get; set; } = 0m;
    public decimal CostoTotalRepuestos { get; set; } = 0m;

    // Propiedades de navegación
    public virtual Equipo? Equipo { get; set; }
    public virtual Ticket? Ticket { get; set; }
    public virtual Usuario? Tecnico { get; set; }
    public virtual ICollection<DetalleMantenimiento> Detalles { get; set; } = new List<DetalleMantenimiento>();
    public virtual ICollection<Adjunto> Adjuntos { get; set; } = new List<Adjunto>();

    public Mantenimiento()
    {
    }

    public Mantenimiento(int equipoId, int tecnicoId, TipoMantenimiento tipo, string diagnostico, decimal horas)
    {
        EquipoId = equipoId;
        TecnicoId = tecnicoId;
        Tipo = tipo;
        Diagnostico = diagnostico;
        HorasTrabajo = horas;
        Fecha = DateTime.UtcNow;
    }

    public decimal CalcularCostoTotal(List<DetalleMantenimiento> detalles)
    {
        CostoTotalRepuestos = detalles.Sum(d => d.GetSubtotal());
        return CostoTotalRepuestos;
    }

    public void FinalizarIntervencion()
    {
        if (Detalles.Count > 0)
        {
            CalcularCostoTotal(Detalles.ToList());
        }
    }
}

