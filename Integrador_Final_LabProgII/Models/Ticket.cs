using ITManager.Models.Enums;

namespace ITManager.Models;

public class Ticket
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public PrioridadTicket Prioridad { get; set; } = PrioridadTicket.MEDIA;
    public EstadoTicket Estado { get; set; } = EstadoTicket.ABIERTO;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaCierre { get; set; }
    public int UsuarioSolicitanteId { get; set; }
    public int EquipoId { get; set; }

    // Propiedades de navegación
    public virtual Usuario? UsuarioSolicitante { get; set; }
    public virtual Equipo? Equipo { get; set; }
    public virtual ICollection<Asignacion> Asignaciones { get; set; } = new List<Asignacion>();
    public virtual ICollection<ComentarioTicket> Comentarios { get; set; } = new List<ComentarioTicket>();
    public virtual ICollection<Mantenimiento> Mantenimientos { get; set; } = new List<Mantenimiento>();
    public virtual ICollection<Adjunto> Adjuntos { get; set; } = new List<Adjunto>();

    public Ticket()
    {
    }

    public Ticket(int id, string codigo, string titulo, string descripcion, PrioridadTicket prioridad, int solicitanteId, int equipoId)
    {
        Id = id;
        Codigo = codigo;
        Titulo = titulo;
        Descripcion = descripcion;
        Prioridad = prioridad;
        UsuarioSolicitanteId = solicitanteId;
        EquipoId = equipoId;
        FechaCreacion = DateTime.UtcNow;
        Estado = EstadoTicket.ABIERTO;
    }

    public void CambiarEstado(EstadoTicket nuevoEstado)
    {
        Estado = nuevoEstado;
        if (nuevoEstado is EstadoTicket.RESUELTO or EstadoTicket.CERRADO && FechaCierre == null)
        {
            FechaCierre = DateTime.UtcNow;
        }
    }

    public void Cerrar()
    {
        Estado = EstadoTicket.CERRADO;
        FechaCierre = DateTime.UtcNow;
    }

    public bool EstaVencido(int diasMaximos)
    {
        if (Estado is EstadoTicket.RESUELTO or EstadoTicket.CERRADO)
        {
            return false;
        }
        return (DateTime.UtcNow - FechaCreacion).TotalDays > diasMaximos;
    }
}

