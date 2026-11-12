using ITManager.Data;
using ITManager.Models;
using ITManager.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITManager.Controllers
{
    [Authorize]
    public class AdjuntosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

        public AdjuntosController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile archivo, int? ticketId = null, int? mantenimientoId = null)
        {
            if (archivo == null || archivo.Length == 0)
            {
                TempData["ErrorMessage"] = "Debe seleccionar un archivo válido.";
                return RedirigirRetorno(ticketId, mantenimientoId);
            }

            if (archivo.Length > MaxFileSize)
            {
                TempData["ErrorMessage"] = "El tamaño máximo de archivo permitido es de 5 MB.";
                return RedirigirRetorno(ticketId, mantenimientoId);
            }

            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
            {
                TempData["ErrorMessage"] = "Solo se permiten imágenes (.jpg, .jpeg, .png) o documentos (.pdf).";
                return RedirigirRetorno(ticketId, mantenimientoId);
            }

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileNameUnico = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileNameUnico);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            var adjunto = new Adjunto
            {
                NombreOriginal = Path.GetFileName(archivo.FileName),
                RutaArchivo = $"/uploads/{fileNameUnico}",
                MimeType = archivo.ContentType,
                Tipo = extension == ".pdf" ? TipoAdjunto.REPORTE_PDF : TipoAdjunto.IMAGEN_EVIDENCIA,
                FechaSubida = DateTime.UtcNow,
                TicketId = ticketId,
                MantenimientoId = mantenimientoId
            };

            await _context.Adjuntos.AddAsync(adjunto);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Archivo '{adjunto.NombreOriginal}' subido correctamente.";
            return RedirigirRetorno(ticketId, mantenimientoId);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var adjunto = await _context.Adjuntos.FindAsync(id);
            if (adjunto == null)
            {
                return NotFound();
            }

            var ticketId = adjunto.TicketId;
            var mantenimientoId = adjunto.MantenimientoId;

            // Eliminar archivo físico de disco
            var relativePath = adjunto.RutaArchivo.TrimStart('/');
            var fullPath = Path.Combine(_environment.WebRootPath, relativePath.Replace('/', Path.DirectorySeparatorChar));

            if (System.IO.File.Exists(fullPath))
            {
                try
                {
                    System.IO.File.Delete(fullPath);
                }
                catch
                {
                    // No frenar borrado lógico si el archivo ya no existe
                }
            }

            _context.Adjuntos.Remove(adjunto);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Archivo adjunto eliminado correctamente.";
            return RedirigirRetorno(ticketId, mantenimientoId);
        }

        private IActionResult RedirigirRetorno(int? ticketId, int? mantenimientoId)
        {
            if (ticketId.HasValue)
            {
                return RedirectToAction("Details", "Tickets", new { id = ticketId.Value });
            }
            if (mantenimientoId.HasValue)
            {
                return RedirectToAction("Details", "Mantenimientos", new { id = mantenimientoId.Value });
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
