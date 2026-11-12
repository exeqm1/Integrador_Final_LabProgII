using ITManager.Models.Enums;
using ITManager.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITManager.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EquiposApiController : ControllerBase
    {
        private readonly IEquipoRepository _equipoRepository;

        public EquiposApiController(IEquipoRepository equipoRepository)
        {
            _equipoRepository = equipoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] int? categoriaId = null,
            [FromQuery] EstadoEquipo? estado = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var paged = await _equipoRepository.GetPagedAsync(page, pageSize, search, categoriaId, estado);

            var items = paged.Items.Select(e => new
            {
                e.Id,
                e.CodigoPatrimonial,
                e.NumeroSerie,
                e.Marca,
                e.Modelo,
                e.FechaAlta,
                Estado = e.Estado.ToString(),
                Categoria = e.Categoria != null ? new { e.Categoria.Id, e.Categoria.Nombre } : null,
                Ubicacion = e.Ubicacion != null ? new { e.Ubicacion.Id, e.Ubicacion.Nombre } : null,
                Custodio = e.UsuarioCustodio != null ? new
                {
                    e.UsuarioCustodio.Id,
                    NombreCompleto = $"{e.UsuarioCustodio.Apellido}, {e.UsuarioCustodio.Nombre}",
                    e.UsuarioCustodio.Email
                } : null
            });

            return Ok(new
            {
                page = paged.Page,
                pageSize = paged.PageSize,
                totalCount = paged.TotalCount,
                totalPages = paged.TotalPages,
                data = items
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var e = await _equipoRepository.GetWithDetailsAsync(id);
            if (e == null)
            {
                return NotFound(new { message = $"Equipo con ID {id} no encontrado." });
            }

            return Ok(new
            {
                e.Id,
                e.CodigoPatrimonial,
                e.NumeroSerie,
                e.Marca,
                e.Modelo,
                e.FechaAlta,
                Estado = e.Estado.ToString(),
                Categoria = e.Categoria != null ? new { e.Categoria.Id, e.Categoria.Nombre } : null,
                Ubicacion = e.Ubicacion != null ? new { e.Ubicacion.Id, e.Ubicacion.Nombre } : null,
                Custodio = e.UsuarioCustodio != null ? new
                {
                    e.UsuarioCustodio.Id,
                    NombreCompleto = $"{e.UsuarioCustodio.Apellido}, {e.UsuarioCustodio.Nombre}",
                    e.UsuarioCustodio.Email
                } : null,
                Tickets = e.Tickets.Select(t => new
                {
                    t.Id,
                    t.Codigo,
                    t.Titulo,
                    Estado = t.Estado.ToString(),
                    Prioridad = t.Prioridad.ToString(),
                    t.FechaCreacion
                }),
                Mantenimientos = e.Mantenimientos.Select(m => new
                {
                    m.Id,
                    m.Fecha,
                    Tipo = m.Tipo.ToString(),
                    m.HorasTrabajo,
                    m.CostoTotalRepuestos
                })
            });
        }
    }
}

