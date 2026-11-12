using System.Security.Claims;
using System.Text.Json;
using ITManager.Models;
using ITManager.Models.Enums;
using ITManager.Repositories.Interfaces;
using ITManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ITManager.Controllers
{
    [Authorize(Roles = "ADMINISTRADOR,TECNICO")]
    public class MantenimientosController : Controller
    {
        private readonly IMantenimientoRepository _mantenimientoRepository;
        private readonly IEquipoRepository _equipoRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IRepository<Repuesto> _repuestoRepository;

        public MantenimientosController(
            IMantenimientoRepository mantenimientoRepository,
            IEquipoRepository equipoRepository,
            ITicketRepository ticketRepository,
            IRepository<Repuesto> repuestoRepository)
        {
            _mantenimientoRepository = mantenimientoRepository;
            _equipoRepository = equipoRepository;
            _ticketRepository = ticketRepository;
            _repuestoRepository = repuestoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, int? equipoId = null)
        {
            var paged = await _mantenimientoRepository.GetPagedAsync(page, pageSize, equipoId);
            var vm = new MantenimientoIndexViewModel
            {
                Mantenimientos = paged.Items,
                Page = paged.Page,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount,
                EquipoId = equipoId
            };
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var mantenimiento = await _mantenimientoRepository.GetWithDetallesAsync(id);
            if (mantenimiento == null)
            {
                return NotFound();
            }
            return View(mantenimiento);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? ticketId = null, int? equipoId = null)
        {
            await CargarViewBagsAsync(equipoId, ticketId);

            var vm = new MantenimientoCreateViewModel
            {
                EquipoId = equipoId ?? 0,
                TicketId = ticketId,
                Tipo = ticketId.HasValue ? TipoMantenimiento.CORRECTIVO : TipoMantenimiento.PREVENTIVO,
                HorasTrabajo = 1.0m
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MantenimientoCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarViewBagsAsync(model.EquipoId, model.TicketId);
                return View(model);
            }

            var userId = ObtenerUsuarioIdActual();

            var detalles = new List<DetalleMantenimiento>();
            if (!string.IsNullOrWhiteSpace(model.DetallesJson))
            {
                try
                {
                    var items = JsonSerializer.Deserialize<List<DetalleItemViewModel>>(model.DetallesJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (items != null)
                    {
                        foreach (var item in items.Where(i => i.Cantidad > 0))
                        {
                            detalles.Add(new DetalleMantenimiento
                            {
                                RepuestoId = item.RepuestoId,
                                Cantidad = item.Cantidad
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Formato inválido de repuestos: " + ex.Message);
                    await CargarViewBagsAsync(model.EquipoId, model.TicketId);
                    return View(model);
                }
            }

            var mantenimiento = new Mantenimiento
            {
                EquipoId = model.EquipoId,
                TicketId = model.TicketId,
                TecnicoId = userId,
                Tipo = model.Tipo,
                Diagnostico = model.Diagnostico.Trim(),
                TareasRealizadas = model.TareasRealizadas.Trim(),
                HorasTrabajo = model.HorasTrabajo,
                Fecha = DateTime.UtcNow
            };

            try
            {
                var id = await _mantenimientoRepository.CreateWithDetallesAsync(mantenimiento, detalles);
                TempData["SuccessMessage"] = $"Orden de mantenimiento #{id} creada exitosamente con descuento transaccional de stock.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await CargarViewBagsAsync(model.EquipoId, model.TicketId);
                return View(model);
            }
        }

        private async Task CargarViewBagsAsync(int? equipoId = null, int? ticketId = null)
        {
            var equipos = await _equipoRepository.GetAllAsync();
            var tickets = await _ticketRepository.GetAllAsync();
            var repuestos = await _repuestoRepository.GetAllAsync();

            ViewBag.Equipos = new SelectList(equipos.Where(e => e.Estado != EstadoEquipo.DE_BAJA).Select(e => new
            {
                e.Id,
                Desc = $"[{e.CodigoPatrimonial}] {e.Marca} {e.Modelo}"
            }), "Id", "Desc", equipoId);

            ViewBag.Tickets = new SelectList(tickets.Where(t => t.Estado != EstadoTicket.CERRADO).Select(t => new
            {
                t.Id,
                Desc = $"[{t.Codigo}] {t.Titulo}"
            }), "Id", "Desc", ticketId);

            ViewBag.RepuestosJson = JsonSerializer.Serialize(repuestos.Select(r => new
            {
                r.Id,
                r.Codigo,
                r.Nombre,
                r.StockActual,
                r.CostoUnitario
            }));

            ViewBag.Tipos = Enum.GetValues<TipoMantenimiento>();
        }

        private int ObtenerUsuarioIdActual()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claim, out var id) ? id : 1;
        }
    }
}
