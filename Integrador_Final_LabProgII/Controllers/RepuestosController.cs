using ITManager.Models;
using ITManager.Repositories.Interfaces;
using ITManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITManager.Controllers
{
    [Authorize(Roles = "ADMINISTRADOR,TECNICO")]
    public class RepuestosController : Controller
    {
        private readonly IRepository<Repuesto> _repuestoRepository;

        public RepuestosController(IRepository<Repuesto> repuestoRepository)
        {
            _repuestoRepository = repuestoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var repuestos = await _repuestoRepository.GetAllAsync();
            return View(repuestos.OrderBy(r => r.Nombre));
        }

        [HttpGet]
        [Authorize(Roles = "ADMINISTRADOR")]
        public IActionResult Create()
        {
            return View(new RepuestoFormViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "ADMINISTRADOR")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RepuestoFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var repuesto = new Repuesto
            {
                Codigo = model.Codigo.Trim().ToUpper(),
                Nombre = model.Nombre.Trim(),
                Descripcion = model.Descripcion?.Trim() ?? string.Empty,
                StockActual = model.StockActual,
                StockMinimo = model.StockMinimo,
                CostoUnitario = model.CostoUnitario
            };

            try
            {
                await _repuestoRepository.CreateAsync(repuesto);
                TempData["SuccessMessage"] = $"Repuesto '{repuesto.Nombre}' creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error al guardar: Verifique que el código no esté duplicado. " + ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "ADMINISTRADOR")]
        public async Task<IActionResult> Edit(int id)
        {
            var r = await _repuestoRepository.GetByIdAsync(id);
            if (r == null) return NotFound();

            var vm = new RepuestoFormViewModel
            {
                Id = r.Id,
                Codigo = r.Codigo,
                Nombre = r.Nombre,
                Descripcion = r.Descripcion,
                StockActual = r.StockActual,
                StockMinimo = r.StockMinimo,
                CostoUnitario = r.CostoUnitario
            };

            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "ADMINISTRADOR")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RepuestoFormViewModel model)
        {
            if (id != model.Id) return BadRequest();

            if (!ModelState.IsValid) return View(model);

            var r = await _repuestoRepository.GetByIdAsync(id);
            if (r == null) return NotFound();

            r.Codigo = model.Codigo.Trim().ToUpper();
            r.Nombre = model.Nombre.Trim();
            r.Descripcion = model.Descripcion?.Trim() ?? string.Empty;
            r.StockActual = model.StockActual;
            r.StockMinimo = model.StockMinimo;
            r.CostoUnitario = model.CostoUnitario;

            await _repuestoRepository.UpdateAsync(r);
            TempData["SuccessMessage"] = $"Repuesto '{r.Nombre}' actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "ADMINISTRADOR,TECNICO")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReponerStock(int id, int cantidad)
        {
            if (cantidad <= 0)
            {
                TempData["ErrorMessage"] = "La cantidad a reponer debe ser mayor a 0.";
                return RedirectToAction(nameof(Index));
            }

            var r = await _repuestoRepository.GetByIdAsync(id);
            if (r == null) return NotFound();

            r.ReponerStock(cantidad);
            await _repuestoRepository.UpdateAsync(r);

            TempData["SuccessMessage"] = $"Se han ingresado {cantidad} unidades a '{r.Nombre}'. Stock actual: {r.StockActual}.";
            return RedirectToAction(nameof(Index));
        }
    }
}
