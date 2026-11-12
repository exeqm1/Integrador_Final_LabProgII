using System.ComponentModel.DataAnnotations;
using ITManager.Models.Enums;

namespace ITManager.ViewModels
{
    public class RepuestoFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El código es obligatorio.")]
        [StringLength(50, ErrorMessage = "El código no puede superar los 50 caracteres.")]
        [Display(Name = "Código de Repuesto")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
        [Display(Name = "Nombre del Repuesto")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "La descripción no puede superar los 255 caracteres.")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Range(0, 100000, ErrorMessage = "El stock actual debe ser mayor o igual a 0.")]
        [Display(Name = "Stock Actual")]
        public int StockActual { get; set; }

        [Range(0, 100000, ErrorMessage = "El stock mínimo debe ser mayor o igual a 0.")]
        [Display(Name = "Stock Mínimo (Alerta)")]
        public int StockMinimo { get; set; } = 5;

        [Range(0.01, 10000000.0, ErrorMessage = "El costo unitario debe ser mayor a 0.")]
        [Display(Name = "Costo Unitario ($)")]
        public decimal CostoUnitario { get; set; }
    }

    public class UsuarioCreateViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mínimo 6 caracteres.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleccione un rol.")]
        public RolUsuario Rol { get; set; } = RolUsuario.EMPLEADO;

        [Required(ErrorMessage = "Seleccione una ubicación.")]
        [Display(Name = "Ubicación")]
        public int UbicacionId { get; set; }
    }

    public class UsuarioEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleccione un rol.")]
        public RolUsuario Rol { get; set; }

        [Required(ErrorMessage = "Seleccione una ubicación.")]
        [Display(Name = "Ubicación")]
        public int UbicacionId { get; set; }

        public bool Activo { get; set; }
    }
}
