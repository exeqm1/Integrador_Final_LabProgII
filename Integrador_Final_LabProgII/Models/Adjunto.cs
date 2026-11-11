using ITManager.Models.Enums;

namespace ITManager.Models;

public class Adjunto
{
    public int Id { get; set; }
    public string RutaArchivo { get; set; } = string.Empty;
    public string NombreOriginal { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long PesoBytes { get; set; } = 0;
    public TipoAdjunto Tipo { get; set; } = TipoAdjunto.IMAGEN_EVIDENCIA;
    public DateTime FechaSubida { get; set; } = DateTime.UtcNow;
    public int? TicketId { get; set; }
    public int? MantenimientoId { get; set; }

    // Propiedades de navegación
    public virtual Ticket? Ticket { get; set; }
    public virtual Mantenimiento? Mantenimiento { get; set; }

    public Adjunto()
    {
    }

    public Adjunto(string ruta, string nombreOriginal, string mime, long peso)
    {
        RutaArchivo = ruta;
        NombreOriginal = nombreOriginal;
        MimeType = mime;
        PesoBytes = peso;
        FechaSubida = DateTime.UtcNow;
    }

    public string GetExtension()
    {
        return Path.GetExtension(NombreOriginal).ToLowerInvariant();
    }
}

