using ITManager.Models;

namespace ITManager.Repositories.Interfaces;

public interface IMantenimientoRepository : IRepository<Mantenimiento>
{
    Task<PagedResult<Mantenimiento>> GetPagedAsync(int page, int pageSize, int? equipoId = null, int? tecnicoId = null);
    Task<Mantenimiento?> GetWithDetallesAsync(int id);
    Task<int> CreateWithDetallesAsync(Mantenimiento mantenimiento, IEnumerable<DetalleMantenimiento> detalles);
    Task<IEnumerable<Mantenimiento>> GetByEquipoAsync(int equipoId);
    Task<IEnumerable<Mantenimiento>> GetByTicketAsync(int ticketId);
}
