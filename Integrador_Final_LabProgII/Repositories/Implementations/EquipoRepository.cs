using ITManager.Data;
using ITManager.Models;
using ITManager.Models.Enums;
using ITManager.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ITManager.Repositories.Implementations;

public class EquipoRepository : Repository<Equipo>, IEquipoRepository
{
    public EquipoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PagedResult<Equipo>> GetPagedAsync(int page, int pageSize, string? searchTerm = null, int? categoriaId = null, EstadoEquipo? estado = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _dbSet
            .Include(e => e.Categoria)
            .Include(e => e.Ubicacion)
            .Include(e => e.UsuarioCustodio)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(e =>
                e.CodigoPatrimonial.ToLower().Contains(term) ||
                e.NumeroSerie.ToLower().Contains(term) ||
                e.Marca.ToLower().Contains(term) ||
                e.Modelo.ToLower().Contains(term));
        }

        if (categoriaId.HasValue)
        {
            query = query.Where(e => e.CategoriaId == categoriaId.Value);
        }

        if (estado.HasValue)
        {
            query = query.Where(e => e.Estado == estado.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(e => e.FechaAlta)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Equipo>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<IEnumerable<Equipo>> SearchAsync(string term, int maxResults = 10)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return Enumerable.Empty<Equipo>();
        }

        var cleanTerm = term.Trim().ToLower();

        return await _dbSet
            .Include(e => e.Categoria)
            .Include(e => e.Ubicacion)
            .AsNoTracking()
            .Where(e =>
                e.CodigoPatrimonial.ToLower().Contains(cleanTerm) ||
                e.NumeroSerie.ToLower().Contains(cleanTerm) ||
                e.Marca.ToLower().Contains(cleanTerm) ||
                e.Modelo.ToLower().Contains(cleanTerm))
            .OrderBy(e => e.CodigoPatrimonial)
            .Take(maxResults)
            .ToListAsync();
    }

    public async Task<IEnumerable<Equipo>> GetByUsuarioCustodioAsync(int usuarioId)
    {
        return await _dbSet
            .Include(e => e.Categoria)
            .Include(e => e.Ubicacion)
            .AsNoTracking()
            .Where(e => e.UsuarioCustodioId == usuarioId)
            .OrderBy(e => e.CodigoPatrimonial)
            .ToListAsync();
    }

    public async Task<Equipo?> GetWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(e => e.Categoria)
            .Include(e => e.Ubicacion)
            .Include(e => e.UsuarioCustodio)
            .Include(e => e.Tickets.OrderByDescending(t => t.FechaCreacion))
                .ThenInclude(t => t.UsuarioSolicitante)
            .Include(e => e.Mantenimientos.OrderByDescending(m => m.Fecha))
                .ThenInclude(m => m.Tecnico)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
}
