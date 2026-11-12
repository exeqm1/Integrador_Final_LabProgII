using System.Security.Claims;
using ITManager.Data;
using ITManager.Models;
using ITManager.Models.Enums;
using ITManager.Repositories.Interfaces;
using ITManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ITManager.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IEquipoRepository _equipoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public TicketsController(
            ITicketRepository ticketRepository,
            IEquipoRepository equipoRepository,
            IUsuarioRepository usuarioRepository,
            AppDbContext context,
            IWebHostEnvironment environment)
        {
            _ticketRepository = ticketRepository;
            _equipoRepository = equipoRepository;
            _usuarioRepository = usuarioRepository;
            _context = context;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, EstadoTicket? estado = null, PrioridadTicket? prioridad = null, bool soloMisTickets = false)
        {
            var userId = ObtenerUsuarioIdActual();
            var esEmpleado = User.IsInRole("EMPLEADO");

            int? solicitanteFiltro = null;
            if (esEmpleado || soloMisTickets)
            {
                solicitanteFiltro = userId;
                soloMisTickets = true;
            }

            var pagedResult = await _ticketRepository.GetPagedAsync(page, pageSize, estado, prioridad, solicitanteFiltro);

            ViewBag.Estados = Enum.GetValues<EstadoTicket>();
            ViewBag.Prioridades = Enum.GetValues<PrioridadTicket>();

            var vm = new TicketIndexViewModel
            {
                Tickets = pagedResult.Items,
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                TotalCount = pagedResult.TotalCount,
                Estado = estado,
                Prioridad = prioridad,
                SoloMisTickets = soloMisTickets
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> MisTickets(int page = 1, int pageSize = 10, EstadoTicket? estado = null, PrioridadTicket? prioridad = null)
        {
            return await Index(page, pageSize, estado, prioridad, soloMisTickets: true);
        }

        [HttpGet]
        [Authorize(Roles = "TECNICO")]
        public async Task<IActionResult> Asignados(int page = 1, int pageSize = 10, EstadoTicket? estado = null, PrioridadTicket? prioridad = null)
        {
            var userId = ObtenerUsuarioIdActual();
            var pagedResult = await _ticketRepository.GetPagedAsync(page, pageSize, estado, prioridad, solicitanteId: null, tecnicoAsignadoId: userId);

            ViewBag.Estados = Enum.GetValues<EstadoTicket>();
            ViewBag.Prioridades = Enum.GetValues<PrioridadTicket>();

            var vm = new TicketIndexViewModel
            {
                Tickets = pagedResult.Items,
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                TotalCount = pagedResult.TotalCount,
                Estado = estado,
                Prioridad = prioridad,
                SoloMisTickets = false
            };

            return View("Index", vm);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? equipoId = null)
        {
            var userId = ObtenerUsuarioIdActual();
            var esEmpleado = User.IsInRole("EMPLEADO");

            // Si es empleado, solo permitimos reportar sobre sus equipos asignados o en general
            var equipos = await _equipoRepository.GetAllAsync();
            var listaEquipos = esEmpleado
                ? equipos.Where(e => e.Estado != EstadoEquipo.DE_BAJA && (e.UsuarioCustodioId == userId || e.UsuarioCustodioId == null))
                : equipos.Where(e => e.Estado != EstadoEquipo.DE_BAJA);

            ViewBag.Equipos = new SelectList(listaEquipos.Select(e => new
            {
                e.Id,
                Descripcion = $"[{e.CodigoPatrimonial}] {e.Marca} {e.Modelo} (S/N: {e.NumeroSerie})"
            }), "Id", "Descripcion", equipoId);

            ViewBag.Prioridades = Enum.GetValues<PrioridadTicket>();

            return View(new TicketCreateViewModel
            {
                EquipoId = equipoId ?? 0,
                Prioridad = PrioridadTicket.MEDIA
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TicketCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var equipos = await _equipoRepository.GetAllAsync();
                ViewBag.Equipos = new SelectList(equipos.Where(e => e.Estado != EstadoEquipo.DE_BAJA).Select(e => new
                {
                    e.Id,
                    Descripcion = $"[{e.CodigoPatrimonial}] {e.Marca} {e.Modelo} (S/N: {e.NumeroSerie})"
                }), "Id", "Descripcion", model.EquipoId);
                ViewBag.Prioridades = Enum.GetValues<PrioridadTicket>();
                return View(model);
            }

            var userId = ObtenerUsuarioIdActual();

            // Código autogenerado: TK-AÑO-CORRELATIVO
            var anio = DateTime.UtcNow.Year;
            var correlativo = Guid.NewGuid().ToString("N")[..4].ToUpper();
            var codigoGenerado = $"TK-{anio}-{correlativo}";

            var ticket = new Ticket
            {
                Codigo = codigoGenerado,
                Titulo = model.Titulo.Trim(),
                Descripcion = model.Descripcion.Trim(),
                Prioridad = model.Prioridad,
                Estado = EstadoTicket.ABIERTO,
                FechaCreacion = DateTime.UtcNow,
                EquipoId = model.EquipoId,
                UsuarioSolicitanteId = userId
            };

            await _ticketRepository.CreateAsync(ticket);

            // Si subió archivo adjunto en la creación, lo persistimos físicamente
            if (model.ArchivoAdjunto != null && model.ArchivoAdjunto.Length > 0 && model.ArchivoAdjunto.Length <= 5 * 1024 * 1024)
            {
                var ext = Path.GetExtension(model.ArchivoAdjunto.FileName).ToLowerInvariant();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
                if (allowed.Contains(ext))
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}{ext}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ArchivoAdjunto.CopyToAsync(stream);
                    }

                    var adjunto = new Adjunto
                    {
                        NombreOriginal = Path.GetFileName(model.ArchivoAdjunto.FileName),
                        RutaArchivo = $"/uploads/{uniqueFileName}",
                        MimeType = model.ArchivoAdjunto.ContentType,
                        Tipo = ext == ".pdf" ? TipoAdjunto.REPORTE_PDF : TipoAdjunto.IMAGEN_EVIDENCIA,
                        FechaSubida = DateTime.UtcNow,
                        TicketId = ticket.Id
                    };

                    await _context.Adjuntos.AddAsync(adjunto);
                    await _context.SaveChangesAsync();
                }
            }

            TempData["SuccessMessage"] = $"Incidente '{ticket.Codigo}' registrado correctamente en la Mesa de Ayuda.";
            return RedirectToAction(nameof(Details), new { id = ticket.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var ticket = await _ticketRepository.GetWithDetailsAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            var userId = ObtenerUsuarioIdActual();
            var esStaff = User.IsInRole("ADMINISTRADOR") || User.IsInRole("TECNICO");

            // Protección IDOR: Si es un empleado, solo puede ver sus propios tickets
            if (!esStaff && ticket.UsuarioSolicitanteId != userId)
            {
                return Forbid();
            }

            if (esStaff)
            {
                var tecnicos = await _usuarioRepository.GetByRolAsync(RolUsuario.TECNICO);
                var asignacionActiva = ticket.Asignaciones.FirstOrDefault(a => a.Activo);
                ViewBag.Tecnicos = new SelectList(tecnicos.Select(t => new
                {
                    t.Id,
                    Nombre = $"{t.Apellido}, {t.Nombre}"
                }), "Id", "Nombre", asignacionActiva?.TecnicoId);
                ViewBag.Estados = Enum.GetValues<EstadoTicket>();
            }

            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComentario(int ticketId, string mensaje)
        {
            if (string.IsNullOrWhiteSpace(mensaje))
            {
                TempData["ErrorMessage"] = "El comentario no puede estar vacío.";
                return RedirectToAction(nameof(Details), new { id = ticketId });
            }

            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null)
            {
                return NotFound();
            }

            var userId = ObtenerUsuarioIdActual();
            var esStaff = User.IsInRole("ADMINISTRADOR") || User.IsInRole("TECNICO");

            // Protección IDOR en comentarios
            if (!esStaff && ticket.UsuarioSolicitanteId != userId)
            {
                return Forbid();
            }

            var comentario = new ComentarioTicket
            {
                TicketId = ticketId,
                UsuarioId = userId,
                Mensaje = mensaje.Trim(),
                FechaHora = DateTime.UtcNow
            };

            await _ticketRepository.AddComentarioAsync(comentario);
            TempData["SuccessMessage"] = "Comentario añadido a la conversación.";
            return RedirectToAction(nameof(Details), new { id = ticketId });
        }

        [HttpPost]
        [Authorize(Roles = "ADMINISTRADOR,TECNICO")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int ticketId, EstadoTicket nuevoEstado)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null)
            {
                return NotFound();
            }

            ticket.CambiarEstado(nuevoEstado);
            await _ticketRepository.UpdateAsync(ticket);
            TempData["SuccessMessage"] = $"El estado del ticket ha cambiado a {nuevoEstado}.";
            return RedirectToAction(nameof(Details), new { id = ticketId });
        }

        [HttpPost]
        [Authorize(Roles = "ADMINISTRADOR,TECNICO")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarTecnico(int ticketId, int tecnicoId)
        {
            var success = await _ticketRepository.AsignarTecnicoAsync(ticketId, tecnicoId);
            if (!success)
            {
                TempData["ErrorMessage"] = "No fue posible asignar el técnico.";
            }
            else
            {
                TempData["SuccessMessage"] = "Técnico asignado correctamente al incidente.";
            }
            return RedirectToAction(nameof(Details), new { id = ticketId });
        }

        private int ObtenerUsuarioIdActual()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claim, out var id) ? id : 1;
        }
    }
}
