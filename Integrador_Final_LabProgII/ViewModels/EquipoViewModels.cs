using System.ComponentModel.DataAnnotations;
using ITManager.Models.Enums;

namespace ITManager.ViewModels
{
    public class EquipoFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El código patrimonial es obligatorio.")]
        [StringLength(50, ErrorMessage = "El código patrimonial no puede exceder los 50 caracteres.")]
        [Display(Name = "Código Patrimonial")]
        public string CodigoPatrimonial { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de serie es obligatorio.")]
        [StringLength(100, ErrorMessage = "El número de serie no puede exceder los 100 caracteres.")]
        [Display(Name = "Número de Serie")]
        public string NumeroSerie { get; set; } = string.Empty;

        [Required(ErrorMessage = "La marca es obligatoria.")]
        [StringLength(100, ErrorMessage = "La marca no puede exceder los 100 caracteres.")]
        public string Marca { get; set; } = string.Empty;

        [Required(ErrorMessage = "El modelo es obligatorio.")]
        [StringLength(100, ErrorMessage = "El modelo no puede exceder los 100 caracteres.")]
        public string Modelo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "La ubicación es obligatoria.")]
        [Display(Name = "Ubicación")]
        public int UbicacionId { get; set; }

        [Display(Name = "Usuario Custodio (Opcional)")]
        public int? UsuarioCustodioId { get; set; }

        [Display(Name = "Estado")]
        public EstadoEquipo Estado { get; set; } = EstadoEquipo.OPERATIVO;
    }

    public class EquipoIndexViewModel
    {
        public IEnumerable<ITManager.Models.Equipo> Equipos { get; set; } = new List<ITManager.Models.Equipo>();
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public string? SearchTerm { get; set; }
        public int? CategoriaId { get; set; }
        public EstadoEquipo? Estado { get; set; }
    }
}

