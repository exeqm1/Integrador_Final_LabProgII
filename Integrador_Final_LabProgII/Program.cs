using ITManager.Data;
using ITManager.Repositories.Implementations;
using ITManager.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database Context (MySQL Pomelo)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost;Database=it_manager_db;User=root;Password=;";

builder.Services.AddDbContext<AppDbContext>(options =>
{
    // Usa versión 8.0.36 fija para que las herramientas de diseño/migraciones no dependan de que MySQL esté prendido para compilar el modelo
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36)));
});

// Repositories DI
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IEquipoRepository, EquipoRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IMantenimientoRepository, MantenimientoRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
