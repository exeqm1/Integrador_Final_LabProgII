using System.Security.Claims;
using ITManager.Data;
using ITManager.Models.Enums;
using ITManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITManager.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new DashboardViewModel();

            // 1. Métricas de Tickets
            var tickets = await _context.Tickets.AsNoTracking().ToListAsync();
            vm.TotalTicketsAbiertos = tickets.Count(t => t.Estado == EstadoTicket.ABIERTO);
            vm.TotalTicketsEnProceso = tickets.Count(t => t.Estado == EstadoTicket.EN_PROCESO);
            vm.TotalTicketsCriticos = tickets.Count(t => t.Prioridad == PrioridadTicket.CRITICA && t.Estado != EstadoTicket.CERRADO);

            vm.TicketsAbiertos = vm.TotalTicketsAbiertos;
            vm.TicketsEnProceso = vm.TotalTicketsEnProceso;
            vm.TicketsResueltos = tickets.Count(t => t.Estado == EstadoTicket.RESUELTO);
            vm.TicketsCerrados = tickets.Count(t => t.Estado == EstadoTicket.CERRADO);

            // 2. Métricas de Equipos
            var equipos = await _context.Equipos.AsNoTracking().ToListAsync();
            vm.TotalEquiposOperativos = equipos.Count(e => e.Estado == EstadoEquipo.OPERATIVO);
            vm.TotalEquiposEnReparacion = equipos.Count(e => e.Estado == EstadoEquipo.EN_REPARACION);
            vm.TotalEquiposDeBaja = equipos.Count(e => e.Estado == EstadoEquipo.DE_BAJA);

            // 3. Métricas de Repuestos
            var repuestos = await _context.Repuestos.AsNoTracking().ToListAsync();
            vm.TotalRepuestosAgotados = repuestos.Count(r => r.StockActual == 0);
            vm.TotalRepuestosCriticos = repuestos.Count(r => r.StockActual > 0 && r.StockActual <= r.StockMinimo);

            // 4. Mantenimientos por Categoría de Equipo
            var mantenimientos = await _context.Mantenimientos
                .Include(m => m.Equipo)
                    .ThenInclude(e => e!.Categoria)
                .AsNoTracking()
                .ToListAsync();

            var categorias = await _context.Categorias.AsNoTracking().ToListAsync();
            foreach (var cat in categorias)
            {
                vm.CategoriasLabels.Add(cat.Nombre);
                vm.MantenimientosPorCategoria.Add(mantenimientos.Count(m => m.Equipo?.CategoriaId == cat.Id));
            }

            return View(vm);
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
