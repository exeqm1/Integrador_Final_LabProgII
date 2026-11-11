namespace ITManager.Models;

public class Categoria
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;

    // Propiedades de navegación
    public virtual ICollection<Equipo> Equipos { get; set; } = new List<Equipo>();

    public Categoria()
    {
    }

    public Categoria(int id, string nombre, string descripcion)
    {
        Id = id;
        Nombre = nombre;
        Descripcion = descripcion;
    }
}

