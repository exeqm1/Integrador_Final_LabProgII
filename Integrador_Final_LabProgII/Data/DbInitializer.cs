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
            await context.Database.EnsureCreatedAsync();

            // 1. Usuarios con nombres y apellidos profesionales reales
            var admin = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == "admin@itmanager.com");
            if (admin == null)
            {
                admin = new Usuario
                {
                    Nombre = "Alejandro",
                    Apellido = "Vargas",
                    Email = "admin@itmanager.com",
                    PasswordHash = hasher.HashPassword("Admin123!*"),
                    Rol = RolUsuario.ADMINISTRADOR,
                    Activo = true,
                    UbicacionId = 1,
                    AvatarUrl = "/img/avatars/default.png"
                };
                await context.Usuarios.AddAsync(admin);
            }
            else
            {
                admin.Nombre = "Alejandro";
                admin.Apellido = "Vargas";
            }

            var tecnico = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == "tecnico@itmanager.com");
            if (tecnico == null)
            {
                tecnico = new Usuario
                {
                    Nombre = "Martín",
                    Apellido = "Gómez",
                    Email = "tecnico@itmanager.com",
                    PasswordHash = hasher.HashPassword("Admin123!*"),
                    Rol = RolUsuario.TECNICO,
                    Activo = true,
                    UbicacionId = 1,
                    AvatarUrl = "/img/avatars/default.png"
                };
                await context.Usuarios.AddAsync(tecnico);
            }
            else
            {
                tecnico.Nombre = "Martín";
                tecnico.Apellido = "Gómez";
            }

            var empleado = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == "empleado@itmanager.com");
            if (empleado == null)
            {
                empleado = new Usuario
                {
                    Nombre = "Laura",
                    Apellido = "Sánchez",
                    Email = "empleado@itmanager.com",
                    PasswordHash = hasher.HashPassword("Admin123!*"),
                    Rol = RolUsuario.EMPLEADO,
                    Activo = true,
                    UbicacionId = 2,
                    AvatarUrl = "/img/avatars/default.png"
                };
                await context.Usuarios.AddAsync(empleado);
            }
            else
            {
                empleado.Nombre = "Laura";
                empleado.Apellido = "Sánchez";
            }

            // Usuario adicional: segundo empleado
            var empleado2 = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == "f.herrera@itmanager.com");
            if (empleado2 == null)
            {
                empleado2 = new Usuario
                {
                    Nombre = "Federico",
                    Apellido = "Herrera",
                    Email = "f.herrera@itmanager.com",
                    PasswordHash = hasher.HashPassword("Admin123!*"),
                    Rol = RolUsuario.EMPLEADO,
                    Activo = true,
                    UbicacionId = 1,
                    AvatarUrl = "/img/avatars/default.png"
                };
                await context.Usuarios.AddAsync(empleado2);
            }

            await context.SaveChangesAsync();

            // 2. Equipos variados
            if (await context.Equipos.CountAsync() < 5)
            {
                if (!await context.Equipos.AnyAsync(e => e.CodigoPatrimonial == "NB-CORP-001"))
                {
                    await context.Equipos.AddAsync(new Equipo
                    {
                        CodigoPatrimonial = "NB-CORP-001",
                        NumeroSerie = "SN-LENOVO-8491",
                        Marca = "Lenovo",
                        Modelo = "ThinkPad T14 Gen 3",
                        CategoriaId = 1,
                        UbicacionId = 1,
                        UsuarioCustodioId = empleado.Id,
                        Estado = EstadoEquipo.OPERATIVO,
                        FechaAlta = DateTime.UtcNow.AddMonths(-8)
                    });
                }

                if (!await context.Equipos.AnyAsync(e => e.CodigoPatrimonial == "PC-LAB-042"))
                {
                    await context.Equipos.AddAsync(new Equipo
                    {
                        CodigoPatrimonial = "PC-LAB-042",
                        NumeroSerie = "SN-DELL-9921",
                        Marca = "Dell",
                        Modelo = "OptiPlex 7090",
                        CategoriaId = 2,
                        UbicacionId = 2,
                        UsuarioCustodioId = null,
                        Estado = EstadoEquipo.EN_REPARACION,
                        FechaAlta = DateTime.UtcNow.AddMonths(-5)
                    });
                }

                if (!await context.Equipos.AnyAsync(e => e.CodigoPatrimonial == "SRV-DATA-001"))
                {
                    await context.Equipos.AddAsync(new Equipo
                    {
                        CodigoPatrimonial = "SRV-DATA-001",
                        NumeroSerie = "SN-HPE-3104",
                        Marca = "HP Enterprise",
                        Modelo = "ProLiant DL380 Gen10",
                        CategoriaId = 3,
                        UbicacionId = 1,
                        UsuarioCustodioId = admin.Id,
                        Estado = EstadoEquipo.OPERATIVO,
                        FechaAlta = DateTime.UtcNow.AddYears(-1)
                    });
                }

                if (!await context.Equipos.AnyAsync(e => e.CodigoPatrimonial == "NB-DIS-015"))
                {
                    await context.Equipos.AddAsync(new Equipo
                    {
                        CodigoPatrimonial = "NB-DIS-015",
                        NumeroSerie = "SN-APPLE-4412",
                        Marca = "Apple",
                        Modelo = "MacBook Pro 16 M2 Pro",
                        CategoriaId = 1,
                        UbicacionId = 1,
                        UsuarioCustodioId = empleado2.Id,
                        Estado = EstadoEquipo.OPERATIVO,
                        FechaAlta = DateTime.UtcNow.AddMonths(-3)
                    });
                }

                if (!await context.Equipos.AnyAsync(e => e.CodigoPatrimonial == "IMP-LOG-003"))
                {
                    await context.Equipos.AddAsync(new Equipo
                    {
                        CodigoPatrimonial = "IMP-LOG-003",
                        NumeroSerie = "SN-ZEBRA-7711",
                        Marca = "Zebra",
                        Modelo = "ZT411 Industrial",
                        CategoriaId = 4,
                        UbicacionId = 3,
                        UsuarioCustodioId = null,
                        Estado = EstadoEquipo.OPERATIVO,
                        FechaAlta = DateTime.UtcNow.AddMonths(-10)
                    });
                }

                await context.SaveChangesAsync();
            }

            // 3. Tickets adicionales
            var eqDell = await context.Equipos.FirstOrDefaultAsync(e => e.CodigoPatrimonial == "PC-LAB-042");
            if (eqDell != null && !await context.Tickets.AnyAsync(t => t.Codigo == "TK-2026-INIT"))
            {
                var ticket1 = new Ticket
                {
                    Codigo = "TK-2026-INIT",
                    Titulo = "Falla de encendido y error de memoria",
                    Descripcion = "El equipo no supera el POST al iniciar, emite secuencia sonora de error de RAM.",
                    Prioridad = PrioridadTicket.CRITICA,
                    Estado = EstadoTicket.EN_PROCESO,
                    FechaCreacion = DateTime.UtcNow.AddDays(-2),
                    EquipoId = eqDell.Id,
                    UsuarioSolicitanteId = empleado.Id
                };
                await context.Tickets.AddAsync(ticket1);
                await context.SaveChangesAsync();

                await context.Asignaciones.AddAsync(new Asignacion
                {
                    TicketId = ticket1.Id,
                    TecnicoId = tecnico.Id,
                    FechaAsignacion = DateTime.UtcNow.AddDays(-1),
                    Activo = true
                });

                await context.ComentariosTickets.AddAsync(new ComentarioTicket
                {
                    TicketId = ticket1.Id,
                    UsuarioId = tecnico.Id,
                    Mensaje = "Equipo ingresado al laboratorio técnico. Se verifican módulos de memoria física.",
                    FechaHora = DateTime.UtcNow.AddDays(-1)
                });

                await context.SaveChangesAsync();
            }

            // Ticket 2: Resuelto
            var eqLenovo = await context.Equipos.FirstOrDefaultAsync(e => e.CodigoPatrimonial == "NB-CORP-001");
            if (eqLenovo != null && !await context.Tickets.AnyAsync(t => t.Codigo == "TK-2026-0002"))
            {
                var ticket2 = new Ticket
                {
                    Codigo = "TK-2026-0002",
                    Titulo = "Reemplazo preventivo de batería y pasta térmica",
                    Descripcion = "La batería presenta degradación superior al 40% y temperaturas elevadas bajo carga.",
                    Prioridad = PrioridadTicket.MEDIA,
                    Estado = EstadoTicket.RESUELTO,
                    FechaCreacion = DateTime.UtcNow.AddDays(-10),
                    EquipoId = eqLenovo.Id,
                    UsuarioSolicitanteId = empleado.Id
                };
                await context.Tickets.AddAsync(ticket2);
                await context.SaveChangesAsync();

                await context.Asignaciones.AddAsync(new Asignacion
                {
                    TicketId = ticket2.Id,
                    TecnicoId = tecnico.Id,
                    FechaAsignacion = DateTime.UtcNow.AddDays(-9),
                    Activo = true
                });

                await context.ComentariosTickets.AddAsync(new ComentarioTicket
                {
                    TicketId = ticket2.Id,
                    UsuarioId = tecnico.Id,
                    Mensaje = "Mantenimiento preventivo completado. Módulo de batería reemplazado satisfactoriamente.",
                    FechaHora = DateTime.UtcNow.AddDays(-8)
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
