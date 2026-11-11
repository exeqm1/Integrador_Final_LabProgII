namespace ITManager.Models;

public class Ubicacion
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Edificio { get; set; } = string.Empty;
    public string PisoOficina { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;

    // Propiedades de navegación
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    public virtual ICollection<Equipo> Equipos { get; set; } = new List<Equipo>();

    public Ubicacion()
    {
    }

    public Ubicacion(int id, string nombre, string edificio, string pisoOficina)
    {
        Id = id;
        Nombre = nombre;
        Edificio = edificio;
        PisoOficina = pisoOficina;
        Activo = true;
    }

    public string GetDescripcionCompleta()
    {
        return $"{Nombre} - Edificio {Edificio}, Piso/Oficina: {PisoOficina}";
    }
}

