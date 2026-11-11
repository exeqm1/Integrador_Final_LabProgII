using ITManager.Models.Enums;

namespace ITManager.Models;

public class Equipo
{
    public int Id { get; set; }
    public string CodigoPatrimonial { get; set; } = string.Empty;
    public string NumeroSerie { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public DateTime FechaAlta { get; set; } = DateTime.UtcNow;
    public EstadoEquipo Estado { get; set; } = EstadoEquipo.OPERATIVO;
    public int CategoriaId { get; set; }
    public int UbicacionId { get; set; }
    public int? UsuarioCustodioId { get; set; }

    // Propiedades de navegación
    public virtual Categoria? Categoria { get; set; }
    public virtual Ubicacion? Ubicacion { get; set; }
    public virtual Usuario? UsuarioCustodio { get; set; }
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public virtual ICollection<Mantenimiento> Mantenimientos { get; set; } = new List<Mantenimiento>();

    public Equipo()
    {
    }

    public Equipo(int id, string codigoPatrimonial, string numeroSerie, string marca, string modelo, int categoriaId, int ubicacionId)
    {
        Id = id;
        CodigoPatrimonial = codigoPatrimonial;
        NumeroSerie = numeroSerie;
        Marca = marca;
        Modelo = modelo;
        CategoriaId = categoriaId;
        UbicacionId = ubicacionId;
        FechaAlta = DateTime.UtcNow;
        Estado = EstadoEquipo.OPERATIVO;
    }

    public void AsignarCustodio(int usuarioId)
    {
        UsuarioCustodioId = usuarioId;
    }

    public void DesasignarCustodio()
    {
        UsuarioCustodioId = null;
    }

    public void CambiarUbicacion(int nuevaUbicacionId)
    {
        UbicacionId = nuevaUbicacionId;
    }

    public void MarcarEnReparacion()
    {
        Estado = EstadoEquipo.EN_REPARACION;
    }

    public void MarcarOperativo()
    {
        Estado = EstadoEquipo.OPERATIVO;
    }

    public void DarDeBaja()
    {
        Estado = EstadoEquipo.DE_BAJA;
        UsuarioCustodioId = null;
    }
}

