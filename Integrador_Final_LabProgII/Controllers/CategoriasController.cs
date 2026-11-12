using ITManager.Models;
using ITManager.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITManager.Controllers
{
    [Authorize(Roles = "ADMINISTRADOR")]
    public class CategoriasController : Controller
    {
        private readonly IRepository<Categoria> _categoriaRepository;

        public CategoriasController(IRepository<Categoria> categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categorias = await _categoriaRepository.GetAllAsync();
            return View(categorias.OrderBy(c => c.Nombre));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Categoria());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Categoria model)
        {
            if (string.IsNullOrWhiteSpace(model.Nombre))
            {
                ModelState.AddModelError(nameof(model.Nombre), "El nombre de la categoría es obligatorio.");
                return View(model);
            }

            try
            {
                await _categoriaRepository.CreateAsync(model);
                TempData["SuccessMessage"] = $"Categoría '{model.Nombre}' creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error al crear la categoría: " + ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var cat = await _categoriaRepository.GetByIdAsync(id);
            if (cat == null) return NotFound();
            return View(cat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Categoria model)
        {
            if (id != model.Id) return BadRequest();

            if (string.IsNullOrWhiteSpace(model.Nombre))
            {
                ModelState.AddModelError(nameof(model.Nombre), "El nombre es obligatorio.");
                return View(model);
            }

            var cat = await _categoriaRepository.GetByIdAsync(id);
            if (cat == null) return NotFound();

            cat.Nombre = model.Nombre.Trim();
            cat.Descripcion = model.Descripcion?.Trim() ?? string.Empty;

            await _categoriaRepository.UpdateAsync(cat);
            TempData["SuccessMessage"] = $"Categoría '{cat.Nombre}' actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var cat = await _categoriaRepository.GetByIdAsync(id);
            if (cat == null) return NotFound();

            try
            {
                await _categoriaRepository.DeleteAsync(id);
                TempData["SuccessMessage"] = $"Categoría '{cat.Nombre}' eliminada.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = $"No se puede eliminar la categoría '{cat.Nombre}' porque posee equipos asociados en el inventario.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // ENDPOINTS AJAX / JSON PARA MÓDULO REACTIVO VUE.JS
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> GetAllJson()
        {
            var categorias = await _categoriaRepository.GetAllAsync();
            var result = categorias
                .OrderBy(c => c.Nombre)
                .Select(c => new
                {
                    id = c.Id,
                    nombre = c.Nombre,
                    descripcion = c.Descripcion ?? string.Empty
                });
            return Json(new { success = true, data = result });
        }

        [HttpPost]
        public async Task<IActionResult> CreateJson([FromBody] Categoria model)
        {
            if (string.IsNullOrWhiteSpace(model.Nombre))
            {
                return Json(new { success = false, message = "El nombre de la categoría es obligatorio." });
            }

            try
            {
                var nueva = new Categoria
                {
                    Nombre = model.Nombre.Trim(),
                    Descripcion = model.Descripcion?.Trim() ?? string.Empty
                };
                await _categoriaRepository.CreateAsync(nueva);
                return Json(new { success = true, message = "Categoría creada con éxito.", data = nueva });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al crear: " + ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateJson(int id, [FromBody] Categoria model)
        {
            if (string.IsNullOrWhiteSpace(model.Nombre))
            {
                return Json(new { success = false, message = "El nombre es obligatorio." });
            }

            var cat = await _categoriaRepository.GetByIdAsync(id);
            if (cat == null)
            {
                return Json(new { success = false, message = "Categoría no encontrada." });
            }

            cat.Nombre = model.Nombre.Trim();
            cat.Descripcion = model.Descripcion?.Trim() ?? string.Empty;

            try
            {
                await _categoriaRepository.UpdateAsync(cat);
                return Json(new { success = true, message = "Categoría actualizada correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al actualizar: " + ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteJson(int id)
        {
            var cat = await _categoriaRepository.GetByIdAsync(id);
            if (cat == null)
            {
                return Json(new { success = false, message = "Categoría no encontrada." });
            }

            try
            {
                await _categoriaRepository.DeleteAsync(id);
                return Json(new { success = true, message = $"Categoría '{cat.Nombre}' eliminada exitosamente." });
            }
            catch (DbUpdateException)
            {
                return Json(new { success = false, message = $"No se puede eliminar la categoría '{cat.Nombre}' porque posee equipos asociados." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al eliminar: " + ex.Message });
            }
        }
    }
}

