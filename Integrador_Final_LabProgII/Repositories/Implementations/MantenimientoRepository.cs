using ITManager.Data;
using ITManager.Models;
using ITManager.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ITManager.Repositories.Implementations;

public class MantenimientoRepository : Repository<Mantenimiento>, IMantenimientoRepository
{
    public MantenimientoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Mantenimiento?> GetWithDetallesAsync(int id)
    {
        return await _dbSet
            .Include(m => m.Equipo)
                .ThenInclude(e => e!.Categoria)
            .Include(m => m.Ticket)
            .Include(m => m.Tecnico)
            .Include(m => m.Detalles)
                .ThenInclude(d => d.Repuesto)
            .Include(m => m.Adjuntos)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<int> CreateWithDetallesAsync(Mantenimiento mantenimiento, IEnumerable<DetalleMantenimiento> detalles)
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var listaDetalles = detalles.ToList();

                // 1. Validar y descontar stock de repuestos
                foreach (var detalle in listaDetalles)
                {
                    var repuesto = await _context.Repuestos.FindAsync(detalle.RepuestoId);
                    if (repuesto == null)
                    {
                        throw new InvalidOperationException($"El repuesto con ID {detalle.RepuestoId} no existe.");
                    }

                    if (!repuesto.DescontarStock(detalle.Cantidad))
                    {
                        throw new InvalidOperationException($"Stock insuficiente para el repuesto '{repuesto.Nombre}'. Stock disponible: {repuesto.StockActual}, solicitado: {detalle.Cantidad}.");
                    }

                    detalle.CostoHistorico = repuesto.CostoUnitario;
                }

                // 2. Calcular costo total de repuestos y persistir mantenimiento
                mantenimiento.CalcularCostoTotal(listaDetalles);
                await _dbSet.AddAsync(mantenimiento);
                await _context.SaveChangesAsync();

                // 3. Asociar IDs y persistir detalles
                foreach (var detalle in listaDetalles)
                {
                    detalle.MantenimientoId = mantenimiento.Id;
                    await _context.DetallesMantenimientos.AddAsync(detalle);
                }
                await _context.SaveChangesAsync();

                // 4. Si el mantenimiento proviene de un ticket y fue correctivo, actualizar estado si corresponde
                if (mantenimiento.TicketId.HasValue)
                {
                    var ticket = await _context.Tickets.FindAsync(mantenimiento.TicketId.Value);
                    if (ticket != null && ticket.Estado != Models.Enums.EstadoTicket.RESUELTO)
                    {
                        ticket.CambiarEstado(Models.Enums.EstadoTicket.RESUELTO);
                    }
                }

                // 5. El equipo vuelve a estar operativo
                var equipo = await _context.Equipos.FindAsync(mantenimiento.EquipoId);
                if (equipo != null)
                {
                    equipo.MarcarOperativo();
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return mantenimiento.Id;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    public async Task<IEnumerable<Mantenimiento>> GetByEquipoAsync(int equipoId)
    {
        return await _dbSet
            .Include(m => m.Tecnico)
            .Include(m => m.Detalles)
                .ThenInclude(d => d.Repuesto)
            .AsNoTracking()
            .Where(m => m.EquipoId == equipoId)
            .OrderByDescending(m => m.Fecha)
            .ToListAsync();
    }

    public async Task<IEnumerable<Mantenimiento>> GetByTicketAsync(int ticketId)
    {
        return await _dbSet
            .Include(m => m.Tecnico)
            .Include(m => m.Detalles)
                .ThenInclude(d => d.Repuesto)
            .AsNoTracking()
            .Where(m => m.TicketId == ticketId)
            .OrderByDescending(m => m.Fecha)
            .ToListAsync();
    }
}

