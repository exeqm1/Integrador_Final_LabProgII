using ITManager.Models;
using ITManager.Models.Enums;
using ITManager.Repositories.Interfaces;
using ITManager.Services.Interfaces;
using ITManager.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ITManager.Controllers
{
    [Authorize(Roles = "ADMINISTRADOR")]
    public class UsuariosController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IRepository<Ubicacion> _ubicacionRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UsuariosController(
            IUsuarioRepository usuarioRepository,
            IRepository<Ubicacion> ubicacionRepository,
            IPasswordHasher passwordHasher)
        {
            _usuarioRepository = usuarioRepository;
            _ubicacionRepository = ubicacionRepository;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            var ubicaciones = (await _ubicacionRepository.GetAllAsync()).ToDictionary(u => u.Id, u => u.Nombre);
            ViewBag.UbicacionesMap = ubicaciones;

            return View(usuarios.OrderBy(u => u.Apellido).ThenBy(u => u.Nombre));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await CargarUbicacionesAsync();
            ViewBag.Roles = Enum.GetValues<RolUsuario>();
            return View(new UsuarioCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await CargarUbicacionesAsync(model.UbicacionId);
                ViewBag.Roles = Enum.GetValues<RolUsuario>();
                return View(model);
            }

            var existe = await _usuarioRepository.GetByEmailAsync(model.Email);
            if (existe != null)
            {
                ModelState.AddModelError(nameof(model.Email), "El correo electrónico ya se encuentra registrado.");
                await CargarUbicacionesAsync(model.UbicacionId);
                ViewBag.Roles = Enum.GetValues<RolUsuario>();
                return View(model);
            }

            var usuario = new Usuario
            {
                Nombre = model.Nombre.Trim(),
                Apellido = model.Apellido.Trim(),
                Email = model.Email.Trim().ToLower(),
                PasswordHash = _passwordHasher.HashPassword(model.Password),
                Rol = model.Rol,
                UbicacionId = model.UbicacionId,
                AvatarUrl = "/img/avatars/default.png",
                Activo = true
            };

            await _usuarioRepository.CreateAsync(usuario);
            TempData["SuccessMessage"] = $"Usuario '{usuario.Email}' dado de alta exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var u = await _usuarioRepository.GetByIdAsync(id);
            if (u == null) return NotFound();

            var vm = new UsuarioEditViewModel
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                Email = u.Email,
                Rol = u.Rol,
                UbicacionId = u.UbicacionId,
                Activo = u.Activo
            };

            await CargarUbicacionesAsync(u.UbicacionId);
            ViewBag.Roles = Enum.GetValues<RolUsuario>();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UsuarioEditViewModel model)
        {
            if (id != model.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                await CargarUbicacionesAsync(model.UbicacionId);
                ViewBag.Roles = Enum.GetValues<RolUsuario>();
                return View(model);
            }

            var u = await _usuarioRepository.GetByIdAsync(id);
            if (u == null) return NotFound();

            u.Nombre = model.Nombre.Trim();
            u.Apellido = model.Apellido.Trim();
            u.Email = model.Email.Trim().ToLower();
            u.Rol = model.Rol;
            u.UbicacionId = model.UbicacionId;
            u.Activo = model.Activo;

            await _usuarioRepository.UpdateAsync(u);
            TempData["SuccessMessage"] = $"Usuario '{u.Email}' actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(int id, string nuevaPassword)
        {
            if (string.IsNullOrWhiteSpace(nuevaPassword) || nuevaPassword.Length < 6)
            {
                TempData["ErrorMessage"] = "La nueva contraseña debe tener al menos 6 caracteres.";
                return RedirectToAction(nameof(Index));
            }

            var nuevoHash = _passwordHasher.HashPassword(nuevaPassword);
            var success = await _usuarioRepository.UpdatePasswordAsync(id, nuevoHash);

            if (success)
            {
                TempData["SuccessMessage"] = "Contraseña restablecida exitosamente para el usuario.";
            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo actualizar la contraseña.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarUbicacionesAsync(int? selectedId = null)
        {
            var ubicaciones = await _ubicacionRepository.GetAllAsync();
            ViewBag.Ubicaciones = new SelectList(ubicaciones, "Id", "Nombre", selectedId);
        }
    }
}
