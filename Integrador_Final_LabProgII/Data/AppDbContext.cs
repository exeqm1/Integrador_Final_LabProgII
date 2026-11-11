using ITManager.Models;
using ITManager.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace ITManager.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Ubicacion> Ubicaciones => Set<Ubicacion>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Equipo> Equipos => Set<Equipo>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Asignacion> Asignaciones => Set<Asignacion>();
    public DbSet<ComentarioTicket> ComentariosTickets => Set<ComentarioTicket>();
    public DbSet<Mantenimiento> Mantenimientos => Set<Mantenimiento>();
    public DbSet<Repuesto> Repuestos => Set<Repuesto>();
    public DbSet<DetalleMantenimiento> DetallesMantenimientos => Set<DetalleMantenimiento>();
    public DbSet<Adjunto> Adjuntos => Set<Adjunto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ================= Ubicacion =================
        modelBuilder.Entity<Ubicacion>(entity =>
        {
            entity.ToTable("Ubicaciones");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(u => u.Edificio).HasMaxLength(50).IsRequired();
            entity.Property(u => u.PisoOficina).HasMaxLength(50).IsRequired();
        });

        // ================= Categoria =================
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.ToTable("Categorias");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(c => c.Descripcion).HasMaxLength(255);
            entity.HasIndex(c => c.Nombre).IsUnique();
        });

        // ================= Usuario =================
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(u => u.Apellido).HasMaxLength(100).IsRequired();
            entity.Property(u => u.Email).HasMaxLength(150).IsRequired();
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.PasswordHash).HasMaxLength(255).IsRequired();
            entity.Property(u => u.Rol).HasConversion<string>().HasMaxLength(30).IsRequired();
            entity.Property(u => u.AvatarUrl).HasMaxLength(255);

            entity.HasOne(u => u.Ubicacion)
                  .WithMany(ub => ub.Usuarios)
                  .HasForeignKey(u => u.UbicacionId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ================= Equipo =================
        modelBuilder.Entity<Equipo>(entity =>
        {
            entity.ToTable("Equipos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CodigoPatrimonial).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.CodigoPatrimonial).IsUnique();
            entity.Property(e => e.NumeroSerie).HasMaxLength(100).IsRequired();
            entity.HasIndex(e => e.NumeroSerie).IsUnique();
            entity.Property(e => e.Marca).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Modelo).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Estado).HasConversion<string>().HasMaxLength(30).IsRequired();

            entity.HasOne(e => e.Categoria)
                  .WithMany(c => c.Equipos)
                  .HasForeignKey(e => e.CategoriaId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Ubicacion)
                  .WithMany(u => u.Equipos)
                  .HasForeignKey(e => e.UbicacionId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.UsuarioCustodio)
                  .WithMany(u => u.EquiposCustodiados)
                  .HasForeignKey(e => e.UsuarioCustodioId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // ================= Ticket =================
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("Tickets");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Codigo).HasMaxLength(50).IsRequired();
            entity.HasIndex(t => t.Codigo).IsUnique();
            entity.Property(t => t.Titulo).HasMaxLength(150).IsRequired();
            entity.Property(t => t.Descripcion).HasColumnType("TEXT").IsRequired();
            entity.Property(t => t.Prioridad).HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.Property(t => t.Estado).HasConversion<string>().HasMaxLength(20).IsRequired();

            entity.HasOne(t => t.UsuarioSolicitante)
                  .WithMany(u => u.TicketsSolicitados)
                  .HasForeignKey(t => t.UsuarioSolicitanteId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Equipo)
                  .WithMany(e => e.Tickets)
                  .HasForeignKey(t => t.EquipoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ================= Asignacion =================
        modelBuilder.Entity<Asignacion>(entity =>
        {
            entity.ToTable("Asignaciones");
            entity.HasKey(a => a.Id);

            entity.HasOne(a => a.Ticket)
                  .WithMany(t => t.Asignaciones)
                  .HasForeignKey(a => a.TicketId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(a => a.Tecnico)
                  .WithMany(u => u.Asignaciones)
                  .HasForeignKey(a => a.TecnicoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ================= ComentarioTicket =================
        modelBuilder.Entity<ComentarioTicket>(entity =>
        {
            entity.ToTable("ComentariosTickets");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Mensaje).HasColumnType("TEXT").IsRequired();

            entity.HasOne(c => c.Ticket)
                  .WithMany(t => t.Comentarios)
                  .HasForeignKey(c => c.TicketId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.Usuario)
                  .WithMany(u => u.Comentarios)
                  .HasForeignKey(c => c.UsuarioId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ================= Mantenimiento =================
        modelBuilder.Entity<Mantenimiento>(entity =>
        {
            entity.ToTable("Mantenimientos");
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Tipo).HasConversion<string>().HasMaxLength(30).IsRequired();
            entity.Property(m => m.Diagnostico).HasColumnType("TEXT").IsRequired();
            entity.Property(m => m.TareasRealizadas).HasColumnType("TEXT").IsRequired();
            entity.Property(m => m.HorasTrabajo).HasPrecision(10, 2);
            entity.Property(m => m.CostoTotalRepuestos).HasPrecision(10, 2);

            entity.HasOne(m => m.Equipo)
                  .WithMany(e => e.Mantenimientos)
                  .HasForeignKey(m => m.EquipoId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.Ticket)
                  .WithMany(t => t.Mantenimientos)
                  .HasForeignKey(m => m.TicketId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(m => m.Tecnico)
                  .WithMany(u => u.MantenimientosEjecutados)
                  .HasForeignKey(m => m.TecnicoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ================= Repuesto =================
        modelBuilder.Entity<Repuesto>(entity =>
        {
            entity.ToTable("Repuestos");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Codigo).HasMaxLength(50).IsRequired();
            entity.HasIndex(r => r.Codigo).IsUnique();
            entity.Property(r => r.Nombre).HasMaxLength(150).IsRequired();
            entity.Property(r => r.Descripcion).HasMaxLength(255);
            entity.Property(r => r.CostoUnitario).HasPrecision(10, 2);
        });

        // ================= DetalleMantenimiento =================
        modelBuilder.Entity<DetalleMantenimiento>(entity =>
        {
            entity.ToTable("DetallesMantenimientos");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.CostoHistorico).HasPrecision(10, 2);

            entity.HasOne(d => d.Mantenimiento)
                  .WithMany(m => m.Detalles)
                  .HasForeignKey(d => d.MantenimientoId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Repuesto)
                  .WithMany(r => r.Detalles)
                  .HasForeignKey(d => d.RepuestoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ================= Adjunto =================
        modelBuilder.Entity<Adjunto>(entity =>
        {
            entity.ToTable("Adjuntos");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.RutaArchivo).HasMaxLength(255).IsRequired();
            entity.Property(a => a.NombreOriginal).HasMaxLength(255).IsRequired();
            entity.Property(a => a.MimeType).HasMaxLength(100).IsRequired();
            entity.Property(a => a.Tipo).HasConversion<string>().HasMaxLength(30).IsRequired();

            entity.HasOne(a => a.Ticket)
                  .WithMany(t => t.Adjuntos)
                  .HasForeignKey(a => a.TicketId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(a => a.Mantenimiento)
                  .WithMany(m => m.Adjuntos)
                  .HasForeignKey(a => a.MantenimientoId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ================= Seed Data Inicial =================
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ubicacion>().HasData(
            new Ubicacion { Id = 1, Nombre = "Sede Central", Edificio = "A", PisoOficina = "Piso 1 - Lab IT", Activo = true },
            new Ubicacion { Id = 2, Nombre = "Campus Norte", Edificio = "B", PisoOficina = "PB - Aulas", Activo = true }
        );

        modelBuilder.Entity<Categoria>().HasData(
            new Categoria { Id = 1, Nombre = "Notebook", Descripcion = "Equipos portátiles corporativos" },
            new Categoria { Id = 2, Nombre = "PC de Escritorio", Descripcion = "Workstations y puestos fijos" },
            new Categoria { Id = 3, Nombre = "Servidor", Descripcion = "Servidores de aplicaciones y bases de datos" },
            new Categoria { Id = 4, Nombre = "Networking", Descripcion = "Switches, routers y puntos de acceso" },
            new Categoria { Id = 5, Nombre = "Impresora", Descripcion = "Impresoras multifunción y térmicas" }
        );

        modelBuilder.Entity<Usuario>().HasData(
            new Usuario
            {
                Id = 1,
                Nombre = "Admin",
                Apellido = "Sistema",
                Email = "admin@itmanager.com",
                PasswordHash = "$2a$11$N.dJ2h8B.a84xQy5zVzE8.j92WJz28w904gZ2Z01pIq81gV92aB1q", // Default hash provisional
                Rol = RolUsuario.ADMINISTRADOR,
                AvatarUrl = "/img/avatars/default.png",
                Activo = true,
                UbicacionId = 1
            },
            new Usuario
            {
                Id = 2,
                Nombre = "Carlos",
                Apellido = "Técnico",
                Email = "tecnico@itmanager.com",
                PasswordHash = "$2a$11$N.dJ2h8B.a84xQy5zVzE8.j92WJz28w904gZ2Z01pIq81gV92aB1q",
                Rol = RolUsuario.TECNICO,
                AvatarUrl = "/img/avatars/default.png",
                Activo = true,
                UbicacionId = 1
            },
            new Usuario
            {
                Id = 3,
                Nombre = "Laura",
                Apellido = "Empleado",
                Email = "empleado@itmanager.com",
                PasswordHash = "$2a$11$N.dJ2h8B.a84xQy5zVzE8.j92WJz28w904gZ2Z01pIq81gV92aB1q",
                Rol = RolUsuario.EMPLEADO,
                AvatarUrl = "/img/avatars/default.png",
                Activo = true,
                UbicacionId = 2
            }
        );

        modelBuilder.Entity<Repuesto>().HasData(
            new Repuesto { Id = 1, Codigo = "REP-SSD-500", Nombre = "Disco SSD 500GB Kingston", Descripcion = "Unidad de estado sólido SATA", StockActual = 15, StockMinimo = 5, CostoUnitario = 45000.00m },
            new Repuesto { Id = 2, Codigo = "REP-RAM-8GB", Nombre = "Memoria RAM DDR4 8GB", Descripcion = "Módulo 3200MHz Crucial", StockActual = 20, StockMinimo = 6, CostoUnitario = 32000.00m },
            new Repuesto { Id = 3, Codigo = "REP-PAS-TER", Nombre = "Pasta Térmica Arctic MX-4", Descripcion = "Compuesto térmico 4g", StockActual = 8, StockMinimo = 3, CostoUnitario = 12000.00m }
        );
    }
}

