using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ITManager.ViewModels
{
    public class UserProfileViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Apellido")]
        public string Apellido { get; set; } = string.Empty;

        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Rol")]
        public string Rol { get; set; } = string.Empty;

        [Display(Name = "Sede / Ubicación")]
        public string UbicacionNombre { get; set; } = string.Empty;

        [Display(Name = "Avatar Actual")]
        public string? AvatarUrl { get; set; }

        [Display(Name = "Subir Nuevo Avatar (JPG, PNG, GIF, WebP - Máx 2MB)")]
        public IFormFile? AvatarFile { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña (opcional)")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string? NuevaPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Nueva Contraseña")]
        [Compare("NuevaPassword", ErrorMessage = "Las contraseñas no coinciden.")]
        public string? ConfirmarPassword { get; set; }
    }
}

