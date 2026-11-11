namespace ITManager.Models;

public class Asignacion
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public int TecnicoId { get; set; }
    public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaBaja { get; set; }
    public bool Activo { get; set; } = true;

    // Propiedades de navegación
    public virtual Ticket? Ticket { get; set; }
    public virtual Usuario? Tecnico { get; set; }

    public Asignacion()
    {
    }

    public Asignacion(int ticketId, int tecnicoId)
    {
        TicketId = ticketId;
        TecnicoId = tecnicoId;
        FechaAsignacion = DateTime.UtcNow;
        Activo = true;
    }

    public void FinalizarAsignacion()
    {
        Activo = false;
        FechaBaja = DateTime.UtcNow;
    }
}

