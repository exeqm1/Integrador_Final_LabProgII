using System.ComponentModel.DataAnnotations;
using ITManager.Models.Enums;

namespace ITManager.ViewModels
{
    public class TicketCreateViewModel
    {
        [Required(ErrorMessage = "Debe seleccionar un equipo.")]
        [Display(Name = "Equipo Afectado")]
        public int EquipoId { get; set; }

        [Required(ErrorMessage = "El título del ticket es obligatorio.")]
        [StringLength(150, ErrorMessage = "El título no puede exceder los 150 caracteres.")]
        [Display(Name = "Título del Incidente")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción de la falla es obligatoria.")]
        [Display(Name = "Descripción del Problema")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleccione la prioridad del ticket.")]
        [Display(Name = "Prioridad")]
        public PrioridadTicket Prioridad { get; set; } = PrioridadTicket.MEDIA;
    }

    public class TicketIndexViewModel
    {
        public IEnumerable<ITManager.Models.Ticket> Tickets { get; set; } = new List<ITManager.Models.Ticket>();
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public EstadoTicket? Estado { get; set; }
        public PrioridadTicket? Prioridad { get; set; }
        public bool SoloMisTickets { get; set; }
    }

    public class TicketCommentViewModel
    {
        [Required(ErrorMessage = "El mensaje no puede estar vacío.")]
        public string Mensaje { get; set; } = string.Empty;
        public int TicketId { get; set; }
    }
}

