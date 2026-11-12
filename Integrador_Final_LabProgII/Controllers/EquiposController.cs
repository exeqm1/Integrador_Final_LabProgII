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
    public class EquiposController : Controller
    {
        private readonly IEquipoRepository _equipoRepository;
        private readonly IRepository<Categoria> _categoriaRepository;
        private readonly IRepository<Ubicacion> _ubicacionRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public EquiposController(
            IEquipoRepository equipoRepository,
            IRepository<Categoria> categoriaRepository,
            IRepository<Ubicacion> ubicacionRepository,
            IUsuarioRepository usuarioRepository)
        {
            _equipoRepository = equipoRepository;
            _categoriaRepository = categoriaRepository;
            _ubicacionRepository = ubicacionRepository;
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? searchTerm = null, int? categoriaId = null, EstadoEquipo? estado = null)
        {
            var pagedResult = await _equipoRepository.GetPagedAsync(page, pageSize, searchTerm, categoriaId, estado);
            var categorias = await _categoriaRepository.GetAllAsync();

            ViewBag.Categorias = new SelectList(categorias, "Id", "Nombre", categoriaId);
            ViewBag.Estados = Enum.GetValues<EstadoEquipo>();

            var vm = new EquipoIndexViewModel
            {
                Equipos = pagedResult.Items,
                Page = pagedResult.Page,
                PageSize = pagedResult.PageSize,
                TotalCount = pagedResult.TotalCount,
                SearchTerm = searchTerm,
                CategoriaId = categoriaId,
                Estado = estado
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var equipo = await _equipoRepository.GetWithDetailsAsync(id);
            if (equipo == null)
            {
                return NotFound();
            }
            return View(equipo);
        }

        [HttpGet]
        [Authorize(Roles = "ADMINISTRADOR,TECNICO")]
        public async Task<IActionResult> Create()
        {
            await CargarViewBagsFormularioAsync();
            return View(new EquipoFormViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "ADMINISTRADOR,TECNICO")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EquipoFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarViewBagsFormularioAsync(model.CategoriaId, model.UbicacionId, model.UsuarioCustodioId);
                return View(model);
            }

            var equipo = new Equipo
            {
                CodigoPatrimonial = model.CodigoPatrimonial.Trim().ToUpper(),
                NumeroSerie = model.NumeroSerie.Trim(),
                Marca = model.Marca.Trim(),
                Modelo = model.Modelo.Trim(),
                CategoriaId = model.CategoriaId,
                UbicacionId = model.UbicacionId,
                UsuarioCustodioId = model.UsuarioCustodioId,
                Estado = model.Estado,
                FechaAlta = DateTime.UtcNow
            };

            await _equipoRepository.CreateAsync(equipo);
            TempData["SuccessMessage"] = $"Equipo '{equipo.CodigoPatrimonial}' registrado con éxito.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "ADMINISTRADOR,TECNICO")]
        public async Task<IActionResult> Edit(int id)
        {
            var equipo = await _equipoRepository.GetByIdAsync(id);
            if (equipo == null)
            {
                return NotFound();
            }

            var vm = new EquipoFormViewModel
            {
                Id = equipo.Id,
                CodigoPatrimonial = equipo.CodigoPatrimonial,
                NumeroSerie = equipo.NumeroSerie,
                Marca = equipo.Marca,
                Modelo = equipo.Modelo,
                CategoriaId = equipo.CategoriaId,
                UbicacionId = equipo.UbicacionId,
                UsuarioCustodioId = equipo.UsuarioCustodioId,
                Estado = equipo.Estado
            };

            await CargarViewBagsFormularioAsync(vm.CategoriaId, vm.UbicacionId, vm.UsuarioCustodioId);
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "ADMINISTRADOR,TECNICO")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EquipoFormViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await CargarViewBagsFormularioAsync(model.CategoriaId, model.UbicacionId, model.UsuarioCustodioId);
                return View(model);
            }

            var equipo = await _equipoRepository.GetByIdAsync(id);
            if (equipo == null)
            {
                return NotFound();
            }

            equipo.CodigoPatrimonial = model.CodigoPatrimonial.Trim().ToUpper();
            equipo.NumeroSerie = model.NumeroSerie.Trim();
            equipo.Marca = model.Marca.Trim();
            equipo.Modelo = model.Modelo.Trim();
            equipo.CategoriaId = model.CategoriaId;
            equipo.UbicacionId = model.UbicacionId;
            equipo.UsuarioCustodioId = model.UsuarioCustodioId;
            equipo.Estado = model.Estado;

            await _equipoRepository.UpdateAsync(equipo);
            TempData["SuccessMessage"] = $"Equipo '{equipo.CodigoPatrimonial}' actualizado con éxito.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "ADMINISTRADOR,TECNICO")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DarDeBaja(int id)
        {
            var equipo = await _equipoRepository.GetByIdAsync(id);
            if (equipo == null)
            {
                return NotFound();
            }

            equipo.DarDeBaja();
            await _equipoRepository.UpdateAsync(equipo);
            TempData["WarningMessage"] = $"Equipo '{equipo.CodigoPatrimonial}' dado de baja del inventario activo.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Search(string term)
        {
            var results = await _equipoRepository.SearchAsync(term, 10);
            var json = results.Select(e => new
            {
                id = e.Id,
                codigo = e.CodigoPatrimonial,
                serie = e.NumeroSerie,
                descripcion = $"{e.Marca} {e.Modelo} - {e.Categoria?.Nombre}",
                estado = e.Estado.ToString()
            });
            return Json(json);
        }

        private async Task CargarViewBagsFormularioAsync(int? categoriaId = null, int? ubicacionId = null, int? custodioId = null)
        {
            var categorias = await _categoriaRepository.GetAllAsync();
            var ubicaciones = await _ubicacionRepository.GetAllAsync();
            var usuarios = await _usuarioRepository.GetAllAsync();

            ViewBag.Categorias = new SelectList(categorias, "Id", "Nombre", categoriaId);
            ViewBag.Ubicaciones = new SelectList(ubicaciones, "Id", "Nombre", ubicacionId);
            ViewBag.Usuarios = new SelectList(usuarios.Select(u => new
            {
                u.Id,
                NombreCompleto = $"{u.Apellido}, {u.Nombre} ({u.Email})"
            }), "Id", "NombreCompleto", custodioId);
            ViewBag.Estados = Enum.GetValues<EstadoEquipo>();
        }
    }
}

