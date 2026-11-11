using ITManager.Models;
using ITManager.Models.Enums;

namespace ITManager.Repositories.Interfaces;

public interface ITicketRepository : IRepository<Ticket>
{
    Task<PagedResult<Ticket>> GetPagedAsync(int page, int pageSize, EstadoTicket? estado = null, PrioridadTicket? prioridad = null, int? solicitanteId = null, int? tecnicoAsignadoId = null);
    Task<Ticket?> GetWithDetailsAsync(int id);
    Task<IEnumerable<ComentarioTicket>> GetComentariosAsync(int ticketId);
    Task<int> AddComentarioAsync(ComentarioTicket comentario);
    Task<bool> AsignarTecnicoAsync(int ticketId, int tecnicoId);
    Task<bool> FinalizarAsignacionAsync(int ticketId, int tecnicoId);
}

