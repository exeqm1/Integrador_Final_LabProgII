using ITManager.Models;
using ITManager.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITManager.Controllers
{
    [Authorize(Roles = "ADMINISTRADOR")]
    public class UbicacionesController : Controller
    {
        private readonly IRepository<Ubicacion> _ubicacionRepository;

        public UbicacionesController(IRepository<Ubicacion> ubicacionRepository)
        {
            _ubicacionRepository = ubicacionRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var ubicaciones = await _ubicacionRepository.GetAllAsync();
            return View(ubicaciones.OrderBy(u => u.Nombre));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Ubicacion { Activo = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ubicacion model)
        {
            if (string.IsNullOrWhiteSpace(model.Nombre) || string.IsNullOrWhiteSpace(model.Edificio) || string.IsNullOrWhiteSpace(model.PisoOficina))
            {
                ModelState.AddModelError(string.Empty, "Nombre, edificio y piso/oficina son obligatorios.");
                return View(model);
            }

            await _ubicacionRepository.CreateAsync(model);
            TempData["SuccessMessage"] = $"Ubicación '{model.Nombre}' creada exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var u = await _ubicacionRepository.GetByIdAsync(id);
            if (u == null) return NotFound();
            return View(u);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ubicacion model)
        {
            if (id != model.Id) return BadRequest();

            if (string.IsNullOrWhiteSpace(model.Nombre) || string.IsNullOrWhiteSpace(model.Edificio) || string.IsNullOrWhiteSpace(model.PisoOficina))
            {
                ModelState.AddModelError(string.Empty, "Nombre, edificio y piso/oficina son obligatorios.");
                return View(model);
            }

            var u = await _ubicacionRepository.GetByIdAsync(id);
            if (u == null) return NotFound();

            u.Nombre = model.Nombre.Trim();
            u.Edificio = model.Edificio.Trim();
            u.PisoOficina = model.PisoOficina.Trim();
            u.Activo = model.Activo;

            await _ubicacionRepository.UpdateAsync(u);
            TempData["SuccessMessage"] = $"Ubicación '{u.Nombre}' actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var u = await _ubicacionRepository.GetByIdAsync(id);
            if (u == null) return NotFound();

            try
            {
                await _ubicacionRepository.DeleteAsync(id);
                TempData["SuccessMessage"] = $"Ubicación '{u.Nombre}' eliminada.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = $"No se puede eliminar '{u.Nombre}' porque posee equipos o usuarios asignados físicamente a esta sede.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
