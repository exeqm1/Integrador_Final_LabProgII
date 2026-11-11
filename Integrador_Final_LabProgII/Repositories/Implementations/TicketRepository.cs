using ITManager.Data;
using ITManager.Models;
using ITManager.Models.Enums;
using ITManager.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ITManager.Repositories.Implementations;

public class TicketRepository : Repository<Ticket>, ITicketRepository
{
    public TicketRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PagedResult<Ticket>> GetPagedAsync(int page, int pageSize, EstadoTicket? estado = null, PrioridadTicket? prioridad = null, int? solicitanteId = null, int? tecnicoAsignadoId = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _dbSet
            .Include(t => t.UsuarioSolicitante)
            .Include(t => t.Equipo)
            .Include(t => t.Asignaciones.Where(a => a.Activo))
                .ThenInclude(a => a.Tecnico)
            .AsNoTracking()
            .AsQueryable();

        if (estado.HasValue)
        {
            query = query.Where(t => t.Estado == estado.Value);
        }

        if (prioridad.HasValue)
        {
            query = query.Where(t => t.Prioridad == prioridad.Value);
        }

        if (solicitanteId.HasValue)
        {
            query = query.Where(t => t.UsuarioSolicitanteId == solicitanteId.Value);
        }

        if (tecnicoAsignadoId.HasValue)
        {
            query = query.Where(t => t.Asignaciones.Any(a => a.Activo && a.TecnicoId == tecnicoAsignadoId.Value));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(t => t.FechaCreacion)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Ticket>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<Ticket?> GetWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(t => t.UsuarioSolicitante)
            .Include(t => t.Equipo)
                .ThenInclude(e => e!.Categoria)
            .Include(t => t.Equipo)
                .ThenInclude(e => e!.Ubicacion)
            .Include(t => t.Asignaciones)
                .ThenInclude(a => a.Tecnico)
            .Include(t => t.Comentarios.OrderBy(c => c.FechaHora))
                .ThenInclude(c => c.Usuario)
            .Include(t => t.Adjuntos)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<ComentarioTicket>> GetComentariosAsync(int ticketId)
    {
        return await _context.ComentariosTickets
            .Include(c => c.Usuario)
            .AsNoTracking()
            .Where(c => c.TicketId == ticketId)
            .OrderBy(c => c.FechaHora)
            .ToListAsync();
    }

    public async Task<int> AddComentarioAsync(ComentarioTicket comentario)
    {
        comentario.FechaHora = DateTime.UtcNow;
        await _context.ComentariosTickets.AddAsync(comentario);
        await _context.SaveChangesAsync();
        return comentario.Id;
    }

    public async Task<bool> AsignarTecnicoAsync(int ticketId, int tecnicoId)
    {
        var ticket = await _dbSet.Include(t => t.Asignaciones).FirstOrDefaultAsync(t => t.Id == ticketId);
        if (ticket == null)
        {
            return false;
        }

        // Finaliza asignaciones previas si no estaban finalizadas
        foreach (var asignacionActiva in ticket.Asignaciones.Where(a => a.Activo && a.TecnicoId != tecnicoId))
        {
            asignacionActiva.FinalizarAsignacion();
        }

        var yaAsignado = ticket.Asignaciones.Any(a => a.Activo && a.TecnicoId == tecnicoId);
        if (!yaAsignado)
        {
            ticket.Asignaciones.Add(new Asignacion(ticketId, tecnicoId));
            if (ticket.Estado == EstadoTicket.ABIERTO)
            {
                ticket.CambiarEstado(EstadoTicket.EN_PROCESO);
            }
        }

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> FinalizarAsignacionAsync(int ticketId, int tecnicoId)
    {
        var asignacion = await _context.Asignaciones
            .FirstOrDefaultAsync(a => a.TicketId == ticketId && a.TecnicoId == tecnicoId && a.Activo);

        if (asignacion == null)
        {
            return false;
        }

        asignacion.FinalizarAsignacion();
        return await _context.SaveChangesAsync() > 0;
    }
}

