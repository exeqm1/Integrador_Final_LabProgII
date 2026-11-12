using ITManager.Models;
using ITManager.Models.Enums;
using ITManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ITManager.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context, IPasswordHasher hasher)
        {
            // 1. Asegurar que la base de datos exista
            await context.Database.EnsureCreatedAsync();

            // 2. Usuarios de prueba adicionales si faltaran
            var admin = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == "admin@itmanager.com");
            if (admin == null)
            {
                admin = new Usuario
                {
                    Nombre = "Admin",
                    Apellido = "Sistema",
                    Email = "admin@itmanager.com",
                    PasswordHash = hasher.HashPassword("Admin123!*"),
                    Rol = RolUsuario.ADMINISTRADOR,
                    Activo = true,
                    UbicacionId = 1,
                    AvatarUrl = "/img/avatars/default.png"
                };
                await context.Usuarios.AddAsync(admin);
            }

            var tecnico = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == "tecnico@itmanager.com");
            if (tecnico == null)
            {
                tecnico = new Usuario
                {
                    Nombre = "Carlos",
                    Apellido = "Técnico",
                    Email = "tecnico@itmanager.com",
                    PasswordHash = hasher.HashPassword("Admin123!*"),
                    Rol = RolUsuario.TECNICO,
                    Activo = true,
                    UbicacionId = 1,
                    AvatarUrl = "/img/avatars/default.png"
                };
                await context.Usuarios.AddAsync(tecnico);
            }

            var empleado = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == "empleado@itmanager.com");
            if (empleado == null)
            {
                empleado = new Usuario
                {
                    Nombre = "Laura",
                    Apellido = "Empleado",
                    Email = "empleado@itmanager.com",
                    PasswordHash = hasher.HashPassword("Admin123!*"),
                    Rol = RolUsuario.EMPLEADO,
                    Activo = true,
                    UbicacionId = 2,
                    AvatarUrl = "/img/avatars/default.png"
                };
                await context.Usuarios.AddAsync(empleado);
            }

            await context.SaveChangesAsync();

            // 3. Equipos de prueba iniciales
            if (!await context.Equipos.AnyAsync())
            {
                var eq1 = new Equipo
                {
                    CodigoPatrimonial = "NB-CORP-001",
                    NumeroSerie = "SN-LENOVO-8491",
                    Marca = "Lenovo",
                    Modelo = "ThinkPad T14 Gen 3",
                    CategoriaId = 1, // Notebook
                    UbicacionId = 1,
                    UsuarioCustodioId = empleado.Id,
                    Estado = EstadoEquipo.OPERATIVO,
                    FechaAlta = DateTime.UtcNow.AddMonths(-6)
                };

                var eq2 = new Equipo
                {
                    CodigoPatrimonial = "PC-LAB-042",
                    NumeroSerie = "SN-DELL-9921",
                    Marca = "Dell",
                    Modelo = "OptiPlex 7090",
                    CategoriaId = 2, // PC Escritorio
                    UbicacionId = 2,
                    UsuarioCustodioId = null,
                    Estado = EstadoEquipo.EN_REPARACION,
                    FechaAlta = DateTime.UtcNow.AddMonths(-3)
                };

                var eq3 = new Equipo
                {
                    CodigoPatrimonial = "SRV-DATA-001",
                    NumeroSerie = "SN-HPE-3104",
                    Marca = "HP Enterprise",
                    Modelo = "ProLiant DL380 Gen10",
                    CategoriaId = 3, // Servidor
                    UbicacionId = 1,
                    UsuarioCustodioId = admin.Id,
                    Estado = EstadoEquipo.OPERATIVO,
                    FechaAlta = DateTime.UtcNow.AddYears(-1)
                };

                await context.Equipos.AddRangeAsync(eq1, eq2, eq3);
                await context.SaveChangesAsync();

                // 4. Ticket de prueba inicial
                var ticket = new Ticket
                {
                    Codigo = "TK-2026-INIT",
                    Titulo = "Falla de encendido y pantalla azul",
                    Descripcion = "La PC de laboratorio no inicia Windows, emite 3 pitidos de memoria RAM.",
                    Prioridad = PrioridadTicket.CRITICA,
                    Estado = EstadoTicket.EN_PROCESO,
                    FechaCreacion = DateTime.UtcNow.AddDays(-2),
                    EquipoId = eq2.Id,
                    UsuarioSolicitanteId = empleado.Id
                };
                await context.Tickets.AddAsync(ticket);
                await context.SaveChangesAsync();

                // 5. Asignación al técnico
                var asignacion = new Asignacion
                {
                    TicketId = ticket.Id,
                    TecnicoId = tecnico.Id,
                    FechaAsignacion = DateTime.UtcNow.AddDays(-1),
                    Activo = true
                };
                await context.Asignaciones.AddAsync(asignacion);

                // Comentario inicial
                var comentario = new ComentarioTicket
                {
                    TicketId = ticket.Id,
                    UsuarioId = tecnico.Id,
                    Mensaje = "Ingresado al taller técnico para reemplazo de módulo de memoria y diagnóstico.",
                    FechaHora = DateTime.UtcNow.AddDays(-1)
                };
                await context.ComentariosTickets.AddAsync(comentario);
                await context.SaveChangesAsync();
            }
        }
    }
}
