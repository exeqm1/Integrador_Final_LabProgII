using System.ComponentModel.DataAnnotations;
using ITManager.Models.Enums;

namespace ITManager.ViewModels
{
    public class MantenimientoCreateViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un equipo.")]
        [Display(Name = "Equipo")]
        public int EquipoId { get; set; }

        [Display(Name = "Ticket Asociado (Opcional)")]
        public int? TicketId { get; set; }

        [Required(ErrorMessage = "Seleccione el tipo de mantenimiento.")]
        public TipoMantenimiento Tipo { get; set; } = TipoMantenimiento.CORRECTIVO;

        [Required(ErrorMessage = "El diagnóstico técnico es obligatorio.")]
        public string Diagnostico { get; set; } = string.Empty;

        [Required(ErrorMessage = "Detalle las tareas técnicas realizadas.")]
        [Display(Name = "Tareas Realizadas")]
        public string TareasRealizadas { get; set; } = string.Empty;

        [Range(0.1, 1000.0, ErrorMessage = "Las horas de trabajo deben ser mayores a 0.")]
        [Display(Name = "Horas de Trabajo")]
        public decimal HorasTrabajo { get; set; } = 1.0m;

        // JSON string serializado desde la grilla interactiva de repuestos en el frontend
        public string? DetallesJson { get; set; }
    }

    public class DetalleItemViewModel
    {
        public int RepuestoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal Subtotal => Cantidad * CostoUnitario;
    }

    public class MantenimientoIndexViewModel
    {
        public IEnumerable<ITManager.Models.Mantenimiento> Mantenimientos { get; set; } = new List<ITManager.Models.Mantenimiento>();
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public int? EquipoId { get; set; }
    }
}

