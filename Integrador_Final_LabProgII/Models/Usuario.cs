using ITManager.Models.Enums;

namespace ITManager.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public RolUsuario Rol { get; set; } = RolUsuario.EMPLEADO;
    public string AvatarUrl { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public int UbicacionId { get; set; }

    // Propiedades de navegación
    public virtual Ubicacion? Ubicacion { get; set; }
    public virtual ICollection<Equipo> EquiposCustodiados { get; set; } = new List<Equipo>();
    public virtual ICollection<Ticket> TicketsSolicitados { get; set; } = new List<Ticket>();
    public virtual ICollection<Asignacion> Asignaciones { get; set; } = new List<Asignacion>();
    public virtual ICollection<ComentarioTicket> Comentarios { get; set; } = new List<ComentarioTicket>();
    public virtual ICollection<Mantenimiento> MantenimientosEjecutados { get; set; } = new List<Mantenimiento>();

    public Usuario()
    {
    }

    public Usuario(int id, string nombre, string apellido, string email, RolUsuario rol, int ubicacionId)
    {
        Id = id;
        Nombre = nombre;
        Apellido = apellido;
        Email = email;
        Rol = rol;
        UbicacionId = ubicacionId;
        Activo = true;
    }

    public void CambiarPassword(string hashNuevo)
    {
        PasswordHash = hashNuevo;
    }

    public void ActualizarAvatar(string rutaArchivo)
    {
        AvatarUrl = rutaArchivo;
    }

    public bool EsAdmin()
    {
        return Rol == RolUsuario.ADMINISTRADOR;
    }

    public bool EsTecnico()
    {
        return Rol == RolUsuario.TECNICO;
    }

    public string GetNombreCompleto()
    {
        return $"{Apellido}, {Nombre}".Trim();
    }
}

