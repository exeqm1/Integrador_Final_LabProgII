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

            // 1. Sedes / Ubicaciones (asegurar al menos 5)
            if (await context.Ubicaciones.CountAsync() < 5)
            {
                var ubicacionesFaltantes = new List<Ubicacion>
                {
                    new Ubicacion { Nombre = "Casa Central", Edificio = "Torre Norte", PisoOficina = "Piso 4 - HelpDesk", Activo = true },
                    new Ubicacion { Nombre = "Sucursal Belgrano", Edificio = "Edificio Belgrano Office", PisoOficina = "Piso 2", Activo = true },
                    new Ubicacion { Nombre = "Centro Logístico Pacheco", Edificio = "Galpón 3", PisoOficina = "Planta Operativa", Activo = true },
                    new Ubicacion { Nombre = "Oficinas Catalinas", Edificio = "Torre Catalinas Sur", PisoOficina = "Piso 14", Activo = true },
                    new Ubicacion { Nombre = "Data Center Secundario", Edificio = "Búnker Córdoba", PisoOficina = "Subsuelo 1", Activo = true }
                };

                foreach (var ub in ubicacionesFaltantes)
                {
                    if (!await context.Ubicaciones.AnyAsync(u => u.Nombre == ub.Nombre))
                    {
                        await context.Ubicaciones.AddAsync(ub);
                    }
                }
                await context.SaveChangesAsync();
            }

            // 2. Categorías (asegurar al menos 5)
            if (await context.Categorias.CountAsync() < 5)
            {
                var categoriasFaltantes = new List<Categoria>
                {
                    new Categoria { Nombre = "Notebooks", Descripcion = "Equipos portátiles corporativos para usuarios y directivos." },
                    new Categoria { Nombre = "PC de Escritorio", Descripcion = "Estaciones fijas para oficinas, recepción y laboratorios." },
                    new Categoria { Nombre = "Servidores y Racks", Descripcion = "Nodos de cómputo, virtualización y storage." },
                    new Categoria { Nombre = "Impresoras y Escáneres", Descripcion = "Equipos multifunción, láser y térmicos industriales." },
                    new Categoria { Nombre = "Networking y Comunicaciones", Descripcion = "Switches core, routers perimetrales y firewalls." }
                };

                foreach (var cat in categoriasFaltantes)
                {
                    if (!await context.Categorias.AnyAsync(c => c.Nombre == cat.Nombre))
                    {
                        await context.Categorias.AddAsync(cat);
                    }
                }
                await context.SaveChangesAsync();
            }

            // 3. Usuarios con nombres y apellidos profesionales reales (al menos 5)
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
                    AvatarUrl = "/img/avatars/admin.jpg"
                };
                await context.Usuarios.AddAsync(admin);
            }
            else
            {
                admin.Nombre = "Alejandro";
                admin.Apellido = "Vargas";
                admin.AvatarUrl = "/img/avatars/admin.jpg";
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
                    AvatarUrl = "/img/avatars/tecnico.jpg"
                };
                await context.Usuarios.AddAsync(tecnico);
            }
            else
            {
                tecnico.Nombre = "Martín";
                tecnico.Apellido = "Gómez";
                tecnico.AvatarUrl = "/img/avatars/tecnico.jpg";
            }

            var tecnico2 = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == "r.navarro@itmanager.com");
            if (tecnico2 == null)
            {
                tecnico2 = new Usuario
                {
                    Nombre = "Rodrigo",
                    Apellido = "Navarro",
                    Email = "r.navarro@itmanager.com",
                    PasswordHash = hasher.HashPassword("Admin123!*"),
                    Rol = RolUsuario.TECNICO,
                    Activo = true,
                    UbicacionId = 1,
                    AvatarUrl = "/img/avatars/tecnico.jpg"
                };
                await context.Usuarios.AddAsync(tecnico2);
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
                    AvatarUrl = "/img/avatars/empleado.jpg"
                };
                await context.Usuarios.AddAsync(empleado);
            }
            else
            {
                empleado.Nombre = "Laura";
                empleado.Apellido = "Sánchez";
                empleado.AvatarUrl = "/img/avatars/empleado.jpg";
            }

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
                    AvatarUrl = "/img/avatars/empleado2.jpg"
                };
                await context.Usuarios.AddAsync(empleado2);
            }
            else
            {
                empleado2.AvatarUrl = "/img/avatars/empleado2.jpg";
            }

            var empleado3 = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == "c.morales@itmanager.com");
            if (empleado3 == null)
            {
                empleado3 = new Usuario
                {
                    Nombre = "Carolina",
                    Apellido = "Morales",
                    Email = "c.morales@itmanager.com",
                    PasswordHash = hasher.HashPassword("Admin123!*"),
                    Rol = RolUsuario.EMPLEADO,
                    Activo = true,
                    UbicacionId = 4,
                    AvatarUrl = "/img/avatars/empleado.jpg"
                };
                await context.Usuarios.AddAsync(empleado3);
            }

            await context.SaveChangesAsync();

            // 4. Repuestos / Inventario (asegurar al menos 5)
            if (await context.Repuestos.CountAsync() < 5)
            {
                var repuestosFaltantes = new List<Repuesto>
                {
                    new Repuesto { Codigo = "REP-NVME-1TB", Nombre = "SSD NVMe M.2 1TB Kingston NV2", Descripcion = "PCIe 4.0 3500MB/s lectura", StockActual = 12, StockMinimo = 4, CostoUnitario = 78.50m },
                    new Repuesto { Codigo = "REP-RAM-16GB-D4", Nombre = "Memoria RAM DDR4 16GB 3200MHz", Descripcion = "Módulo SODIMM para laptops", StockActual = 8, StockMinimo = 5, CostoUnitario = 42.00m },
                    new Repuesto { Codigo = "REP-RAM-32GB-D5", Nombre = "Memoria RAM DDR5 32GB 5600MHz", Descripcion = "Módulo ECC para workstations y servidores", StockActual = 3, StockMinimo = 4, CostoUnitario = 110.00m },
                    new Repuesto { Codigo = "REP-BAT-LEN-T14", Nombre = "Batería Original Lenovo ThinkPad T14", Descripcion = "Batería 3 celdas 52.5Wh 01AV421", StockActual = 5, StockMinimo = 2, CostoUnitario = 95.00m },
                    new Repuesto { Codigo = "REP-COOL-FAN", Nombre = "Cooler Disipador CPU Dell OptiPlex", Descripcion = "Fan blower PWM 4 pines", StockActual = 2, StockMinimo = 3, CostoUnitario = 18.20m },
                    new Repuesto { Codigo = "REP-CAB-ETH-CAT6", Nombre = "Patchcord UTP Cat6 3m", Descripcion = "Cable de red blindado RJ45", StockActual = 25, StockMinimo = 10, CostoUnitario = 4.50m }
                };

                foreach (var rep in repuestosFaltantes)
                {
                    if (!await context.Repuestos.AnyAsync(r => r.Codigo == rep.Codigo))
                    {
                        await context.Repuestos.AddAsync(rep);
                    }
                }
                await context.SaveChangesAsync();
            }

            // 5. Equipos (asegurar al menos 6)
            var listaEquipos = new List<Equipo>
            {
                new Equipo
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
                },
                new Equipo
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
                },
                new Equipo
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
                },
                new Equipo
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
                },
                new Equipo
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
                },
                new Equipo
                {
                    CodigoPatrimonial = "SW-CORE-001",
                    NumeroSerie = "SN-CISCO-8820",
                    Marca = "Cisco",
                    Modelo = "Catalyst 9300 48P",
                    CategoriaId = 5,
                    UbicacionId = 1,
                    UsuarioCustodioId = admin.Id,
                    Estado = EstadoEquipo.OPERATIVO,
                    FechaAlta = DateTime.UtcNow.AddYears(-2)
                }
            };

            foreach (var eq in listaEquipos)
            {
                if (!await context.Equipos.AnyAsync(e => e.CodigoPatrimonial == eq.CodigoPatrimonial))
                {
                    await context.Equipos.AddAsync(eq);
                }
            }
            await context.SaveChangesAsync();

            // 6. Tickets (asegurar al menos 5 tickets con ciclo de vida completo)
            var eq1 = await context.Equipos.FirstAsync(e => e.CodigoPatrimonial == "NB-CORP-001");
            var eq2 = await context.Equipos.FirstAsync(e => e.CodigoPatrimonial == "PC-LAB-042");
            var eq3 = await context.Equipos.FirstAsync(e => e.CodigoPatrimonial == "SRV-DATA-001");
            var eq4 = await context.Equipos.FirstAsync(e => e.CodigoPatrimonial == "NB-DIS-015");
            var eq5 = await context.Equipos.FirstAsync(e => e.CodigoPatrimonial == "IMP-LOG-003");

            var ticketsData = new List<Ticket>
            {
                new Ticket
                {
                    Codigo = "TK-2026-0001",
                    Titulo = "Falla de encendido y error de memoria",
                    Descripcion = "La PC emite secuencia sonora de 3 pitidos largos y no da video en pantalla.",
                    Prioridad = PrioridadTicket.CRITICA,
                    Estado = EstadoTicket.EN_PROCESO,
                    FechaCreacion = DateTime.UtcNow.AddDays(-3),
                    EquipoId = eq2.Id,
                    UsuarioSolicitanteId = empleado.Id
                },
                new Ticket
                {
                    Codigo = "TK-2026-0002",
                    Titulo = "Reemplazo preventivo de batería y pasta térmica",
                    Descripcion = "Autonomía reducida a menos de 45 minutos y temperaturas de CPU sobre 85°C.",
                    Prioridad = PrioridadTicket.MEDIA,
                    Estado = EstadoTicket.RESUELTO,
                    FechaCreacion = DateTime.UtcNow.AddDays(-12),
                    FechaCierre = DateTime.UtcNow.AddDays(-10),
                    EquipoId = eq1.Id,
                    UsuarioSolicitanteId = empleado.Id
                },
                new Ticket
                {
                    Codigo = "TK-2026-0003",
                    Titulo = "Atascamiento recurrente de etiquetas",
                    Descripcion = "El rodillo de tracción resbala con las etiquetas térmicas del depósito central.",
                    Prioridad = PrioridadTicket.ALTA,
                    Estado = EstadoTicket.ABIERTO,
                    FechaCreacion = DateTime.UtcNow.AddHours(-18),
                    EquipoId = eq5.Id,
                    UsuarioSolicitanteId = empleado2.Id
                },
                new Ticket
                {
                    Codigo = "TK-2026-0004",
                    Titulo = "Actualización de firmware de controladora RAID",
                    Descripcion = "Mantenimiento preventivo programado para actualizar el firmware del servidor principal.",
                    Prioridad = PrioridadTicket.BAJA,
                    Estado = EstadoTicket.CERRADO,
                    FechaCreacion = DateTime.UtcNow.AddDays(-20),
                    FechaCierre = DateTime.UtcNow.AddDays(-18),
                    EquipoId = eq3.Id,
                    UsuarioSolicitanteId = admin.Id
                },
                new Ticket
                {
                    Codigo = "TK-2026-0005",
                    Titulo = "Falla de teclado y tecla espaciadora bloqueada",
                    Descripcion = "La tecla de espacio no responde tras contacto accidental con líquido en la oficina.",
                    Prioridad = PrioridadTicket.ALTA,
                    Estado = EstadoTicket.EN_PROCESO,
                    FechaCreacion = DateTime.UtcNow.AddDays(-1),
                    EquipoId = eq4.Id,
                    UsuarioSolicitanteId = empleado2.Id
                }
            };

            foreach (var tk in ticketsData)
            {
                if (!await context.Tickets.AnyAsync(t => t.Codigo == tk.Codigo))
                {
                    await context.Tickets.AddAsync(tk);
                    await context.SaveChangesAsync();

                    // Asignación técnica
                    await context.Asignaciones.AddAsync(new Asignacion
                    {
                        TicketId = tk.Id,
                        TecnicoId = tecnico.Id,
                        FechaAsignacion = tk.FechaCreacion.AddHours(2),
                        Activo = tk.Estado != EstadoTicket.CERRADO
                    });

                    // Comentario técnico
                    await context.ComentariosTickets.AddAsync(new ComentarioTicket
                    {
                        TicketId = tk.Id,
                        UsuarioId = tecnico.Id,
                        Mensaje = $"Incidente recibido y registrado bajo seguimiento técnico por Martín Gómez.",
                        FechaHora = tk.FechaCreacion.AddHours(3)
                    });

                    await context.SaveChangesAsync();
                }
            }

            // 7. Mantenimientos (asegurar al menos 5 registros)
            if (await context.Mantenimientos.CountAsync() < 5)
            {
                var repSSD = await context.Repuestos.FirstOrDefaultAsync(r => r.Codigo == "REP-NVME-1TB");
                var repRAM = await context.Repuestos.FirstOrDefaultAsync(r => r.Codigo == "REP-RAM-16GB-D4");
                var repBAT = await context.Repuestos.FirstOrDefaultAsync(r => r.Codigo == "REP-BAT-LEN-T14");

                var tk1 = await context.Tickets.FirstOrDefaultAsync(t => t.Codigo == "TK-2026-0001");
                var tk2 = await context.Tickets.FirstOrDefaultAsync(t => t.Codigo == "TK-2026-0002");
                var tk4 = await context.Tickets.FirstOrDefaultAsync(t => t.Codigo == "TK-2026-0004");

                var listaMants = new List<Mantenimiento>
                {
                    new Mantenimiento
                    {
                        Tipo = TipoMantenimiento.CORRECTIVO,
                        Fecha = DateTime.UtcNow.AddDays(-2),
                        Diagnostico = "Módulo de RAM defectuoso con múltiples sectores corruptos detectados en MemTest86.",
                        TareasRealizadas = "Reemplazo de módulo DDR4 16GB dañado y limpieza de contactos de slot.",
                        HorasTrabajo = 2.5m,
                        CostoTotalRepuestos = repRAM?.CostoUnitario ?? 42.00m,
                        EquipoId = eq2.Id,
                        TicketId = tk1?.Id,
                        TecnicoId = tecnico.Id
                    },
                    new Mantenimiento
                    {
                        Tipo = TipoMantenimiento.PREVENTIVO,
                        Fecha = DateTime.UtcNow.AddDays(-10),
                        Diagnostico = "Degradación de celda química en batería y acumulación de polvo en disipador.",
                        TareasRealizadas = "Instalación de batería original nueva, recambio de pasta térmica Arctic MX-4 y prueba de estrés.",
                        HorasTrabajo = 1.5m,
                        CostoTotalRepuestos = repBAT?.CostoUnitario ?? 95.00m,
                        EquipoId = eq1.Id,
                        TicketId = tk2?.Id,
                        TecnicoId = tecnico.Id
                    },
                    new Mantenimiento
                    {
                        Tipo = TipoMantenimiento.PREVENTIVO,
                        Fecha = DateTime.UtcNow.AddDays(-18),
                        Diagnostico = "Plan trimestral de actualización de microcódigo y firmware de controladora de discos.",
                        TareasRealizadas = "Flasheo de firmware v5.32 en controladora Smart Array y verificación de volumen RAID 10.",
                        HorasTrabajo = 3.0m,
                        CostoTotalRepuestos = 0m,
                        EquipoId = eq3.Id,
                        TicketId = tk4?.Id,
                        TecnicoId = tecnico.Id
                    },
                    new Mantenimiento
                    {
                        Tipo = TipoMantenimiento.CORRECTIVO,
                        Fecha = DateTime.UtcNow.AddDays(-25),
                        Diagnostico = "Desgaste mecánico de rodillo de arrastre y suciedad de cabezal térmico.",
                        TareasRealizadas = "Limpieza con alcohol isopropílico de cabezal y calibración de sensor de papel.",
                        HorasTrabajo = 1.0m,
                        CostoTotalRepuestos = 0m,
                        EquipoId = eq5.Id,
                        TicketId = null,
                        TecnicoId = tecnico2.Id
                    },
                    new Mantenimiento
                    {
                        Tipo = TipoMantenimiento.PREVENTIVO,
                        Fecha = DateTime.UtcNow.AddDays(-30),
                        Diagnostico = "Reemplazo programado de unidad SSD NVMe por cumplimiento de ciclo TBW.",
                        TareasRealizadas = "Clonación sector a sector e instalación de disco NVMe 1TB Kingston nuevo.",
                        HorasTrabajo = 2.0m,
                        CostoTotalRepuestos = repSSD?.CostoUnitario ?? 78.50m,
                        EquipoId = eq1.Id,
                        TicketId = null,
                        TecnicoId = tecnico.Id
                    }
                };

                await context.Mantenimientos.AddRangeAsync(listaMants);
                await context.SaveChangesAsync();
            }
        }
    }
}
