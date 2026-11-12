using ITManager.Models;
using ITManager.Models.Enums;

namespace ITManager.Repositories.Interfaces;

public interface IEquipoRepository : IRepository<Equipo>
{
    Task<PagedResult<Equipo>> GetPagedAsync(int page, int pageSize, string? searchTerm = null, int? categoriaId = null, EstadoEquipo? estado = null);
    Task<IEnumerable<Equipo>> SearchAsync(string term, int maxResults = 10);
    Task<IEnumerable<Equipo>> GetByUsuarioCustodioAsync(int usuarioId);
    Task<Equipo?> GetWithDetailsAsync(int id);
}
