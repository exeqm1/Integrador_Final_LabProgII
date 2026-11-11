namespace ITManager.Models;

public class ComentarioTicket
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public int UsuarioId { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public DateTime FechaHora { get; set; } = DateTime.UtcNow;

    // Propiedades de navegación
    public virtual Ticket? Ticket { get; set; }
    public virtual Usuario? Usuario { get; set; }

    public ComentarioTicket()
    {
    }

    public ComentarioTicket(int ticketId, int usuarioId, string mensaje)
    {
        TicketId = ticketId;
        UsuarioId = usuarioId;
        Mensaje = mensaje;
        FechaHora = DateTime.UtcNow;
    }
}

