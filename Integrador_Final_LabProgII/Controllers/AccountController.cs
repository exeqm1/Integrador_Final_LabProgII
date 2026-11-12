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
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IUsuarioRepository usuarioRepository,
            IPasswordHasher passwordHasher,
            ILogger<AccountController> logger)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
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
    }
}

