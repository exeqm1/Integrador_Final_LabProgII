using System.Security.Claims;
using ITManager.Models;
using ITManager.Repositories.Interfaces;
using ITManager.Services.Interfaces;
using ITManager.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITManager.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IUsuarioRepository usuarioRepository,
            IPasswordHasher passwordHasher,
            IWebHostEnvironment env,
            ILogger<AccountController> logger)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
            _env = env;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = await _usuarioRepository.GetByEmailAsync(model.Email);

            if (usuario == null || !_passwordHasher.VerifyPassword(model.Password, usuario.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Correo electrónico o contraseña incorrectos.");
                return View(model);
            }

            if (!usuario.Activo)
            {
                ModelState.AddModelError(string.Empty, "El usuario se encuentra inactivo. Comuníquese con el administrador.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellido}".Trim()),
                new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
                new Claim("AvatarUrl", string.IsNullOrEmpty(usuario.AvatarUrl) ? "/img/avatars/default.png" : usuario.AvatarUrl),
                new Claim("UbicacionId", usuario.UbicacionId.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties);

            _logger.LogInformation("Usuario {Email} inició sesión exitosamente con rol {Rol}.", usuario.Email, usuario.Rol);

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("Usuario {Email} cerró sesión.", email);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> LogoutGet()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login");
            }

            var usuario = await _usuarioRepository.GetByIdAsync(userId);
            if (usuario == null)
            {
                return NotFound();
            }

            var model = new UserProfileViewModel
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Rol = usuario.Rol.ToString(),
                UbicacionNombre = usuario.Ubicacion?.Nombre ?? "Sin Asignar",
                AvatarUrl = string.IsNullOrEmpty(usuario.AvatarUrl) ? "/img/avatars/default.png" : usuario.AvatarUrl
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UserProfileViewModel model)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId) || userId != model.Id)
            {
                return Forbid();
            }

            var usuario = await _usuarioRepository.GetByIdAsync(userId);
            if (usuario == null)
            {
                return NotFound();
            }

            // Procesar Avatar
            if (model.AvatarFile != null && model.AvatarFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var ext = Path.GetExtension(model.AvatarFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(ext))
                {
                    ModelState.AddModelError("AvatarFile", "Formato de imagen no permitido (Use JPG, PNG, GIF o WebP).");
                }
                else if (model.AvatarFile.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("AvatarFile", "El avatar no debe superar los 2 MB.");
                }
                else
                {
                    var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "avatars");
                    if (!Directory.Exists(uploadsDir))
                    {
                        Directory.CreateDirectory(uploadsDir);
                    }

                    var uniqueFileName = $"avatar_{usuario.Id}_{Guid.NewGuid()}{ext}";
                    var filePath = Path.Combine(uploadsDir, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.AvatarFile.CopyToAsync(stream);
                    }

                    usuario.AvatarUrl = $"/uploads/avatars/{uniqueFileName}";
                }
            }

            // Procesar Cambio de Contraseña Opcional
            if (!string.IsNullOrWhiteSpace(model.NuevaPassword))
            {
                if (model.NuevaPassword.Length < 6)
                {
                    ModelState.AddModelError("NuevaPassword", "La contraseña debe tener al menos 6 caracteres.");
                }
                else if (model.NuevaPassword != model.ConfirmarPassword)
                {
                    ModelState.AddModelError("ConfirmarPassword", "Las contraseñas no coinciden.");
                }
                else
                {
                    usuario.PasswordHash = _passwordHasher.HashPassword(model.NuevaPassword);
                }
            }

            if (!ModelState.IsValid)
            {
                model.Nombre = usuario.Nombre;
                model.Apellido = usuario.Apellido;
                model.Email = usuario.Email;
                model.Rol = usuario.Rol.ToString();
                model.UbicacionNombre = usuario.Ubicacion?.Nombre ?? "Sin Asignar";
                model.AvatarUrl = string.IsNullOrEmpty(usuario.AvatarUrl) ? "/img/avatars/default.png" : usuario.AvatarUrl;
                return View(model);
            }

            await _usuarioRepository.UpdateAsync(usuario);

            // Actualizar Cookie con el nuevo Avatar
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellido}".Trim()),
                new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
                new Claim("AvatarUrl", string.IsNullOrEmpty(usuario.AvatarUrl) ? "/img/avatars/default.png" : usuario.AvatarUrl),
                new Claim("UbicacionId", usuario.UbicacionId.ToString())
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            TempData["Success"] = "Perfil y avatar actualizados exitosamente.";
            return RedirectToAction("Profile");
        }
    }
}

