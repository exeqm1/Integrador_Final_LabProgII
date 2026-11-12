# IT Manager - Plataforma Integral de Gestión de Activos, Mesa de Ayuda y Mantenimiento

Sistema web corporativo desarrollado en **ASP.NET Core (.NET 10)** con arquitectura limpia por capas y persistencia relacional en **MySQL** a través de **Entity Framework Core (Pomelo)**.

---

## Características Principales

1. **Gestión Patrimonial de Activos (Equipos):**
   - Control de hardware con código patrimonial único, número de serie, marcas, modelos y ubicaciones físicas.
   - Asignación opcional de usuarios custodios y estados operativos (`OPERATIVO`, `EN_REPARACION`, `DE_BAJA`).
   - Paginación del lado del servidor y búsqueda predictiva AJAX.

2. **Mesa de Ayuda (HelpDesk & Tickets):**
   - Reporte de averías e incidentes con código correlativo (`TK-YYYY-XXXX`).
   - Hilo de mensajes y actualizaciones en vivo por ticket.
   - Asignación y reasignación de técnicos con trazabilidad histórica.
   - Control de acceso estricto (RBAC) con protección contra IDOR (los empleados solo ven sus propios tickets).

3. **Taller y Mantenimiento con Deducción Transaccional de Repuestos:**
   - Órdenes de trabajo preventivas y correctivas.
   - Grilla interactiva en el frontend con validación de stock disponible en tiempo real.
   - Transacción ACID en Entity Framework Core (`CreateWithDetallesAsync`):
     - Descuenta el stock del pañol automáticamente (`StockActual >= Cantidad`).
     - Congela el `CostoHistorico` de cada pieza utilizada.
     - Cambia el estado del ticket a `RESUELTO`.
     - Reintegra el equipo a estado `OPERATIVO`.

4. **Pañol de Repuestos:**
   - Control de stock actual vs. stock mínimo con alertas semafóricas (`NORMAL`, `STOCK CRÍTICO`, `AGOTADO`).
   - Módulo de reposición rápida de inventario directo desde listado/detalle.

5. **Seguridad & API REST:**
   - Autenticación dual: Cookies para la interfaz web MVC y JWT Bearer para la API REST.
   - Criptografía segura de contraseñas con PBKDF2 (HMAC-SHA256, salt de 128 bits y 100.000 iteraciones).
   - Endpoints REST para autenticación y consulta externa de inventario (`/api/auth/login`, `/api/equipos`).

6. **Dashboard Ejecutivo en Tiempo Real:**
   - Tarjetas KPI con estado de tickets críticos, activos en taller y quiebres de stock.
   - Gráficos interactivos renderizados con **Chart.js** (distribución de tickets e intervenciones por categoría).

7. **Gestión de Archivos Adjuntos:**
   - Carga multipart de imágenes y PDFs tanto en creación como en detalle de Tickets y Mantenimientos.
   - Almacenamiento seguro en `wwwroot/uploads/` con nombres normalizados mediante GUID y persistencia en tabla `Adjuntos`.

---

## Tecnologías Utilizadas

- **Backend:** C# | .NET 10 | ASP.NET Core MVC | Entity Framework Core 9.0
- **Base de Datos:** MySQL 8.x / MariaDB (vía `Pomelo.EntityFrameworkCore.MySql`)
- **Frontend:** Bootstrap 5.3 Dark Mode | Bootstrap Icons | Chart.js
- **Seguridad:** Cookie Authentication | JWT (JSON Web Tokens) | PBKDF2 Hasher

---

## Estructura de la Solución

```text
Integrador_Final_LabProgII/
├── Controllers/
│   ├── Api/                     # Controladores API REST (Auth y Equipos con JWT)
│   ├── AccountController.cs     # Autenticación MVC, login y logout por Cookies
│   ├── AdjuntosController.cs    # Subida física y eliminación de archivos adjuntos
│   ├── CategoriasController.cs  # CRUD de categorías de equipos
│   ├── EquiposController.cs     # Gestión de activos, paginación y filtros
│   ├── HomeController.cs        # Dashboard ejecutivo con métricas KPI y Chart.js
│   ├── MantenimientosController.cs # Taller, remito técnico y transacciones
│   ├── RepuestosController.cs   # Pañol, control de stock y reposición
│   ├── TicketsController.cs     # Mesa de ayuda, asignación de técnicos e IDOR
│   ├── UbicacionesController.cs # CRUD de sedes y oficinas
│   └── UsuariosController.cs    # Gestión de usuarios, roles y reseteo de claves
├── Data/
│   ├── ApplicationDbContext.cs  # Contexto de EF Core, relaciones Fluent API
│   ├── DbInitializer.cs         # Inicializador y sembrado automático (Seed Data)
│   └── Repositories/            # Patrón Repositorio e interfaces desacopladas
├── Models/                      # Entidades del dominio
├── Services/                    # Hashing de contraseñas (PBKDF2) y utilitarios
├── ViewModels/                  # ViewModels para vistas y reportes tipados
└── Views/                       # Vistas Razor con soporte de roles y modo oscuro
```

---

## Credenciales de Acceso (Seed Data)

El sistema inicializa automáticamente al arrancar las siguientes cuentas de prueba:

| Rol | Correo Electrónico | Contraseña | Permisos / Alcance |
| :--- | :--- | :--- | :--- |
| **Administrador** | `admin@itmanager.com` | `Admin123!*` | Acceso total a todos los módulos y catálogos administrativos |
| **Técnico** | `tecnico@itmanager.com` | `Admin123!*` | Mesa de ayuda, taller de mantenimiento y consulta/reposición de pañol |
| **Empleado** | `empleado@itmanager.com` | `Admin123!*` | Reportar incidentes, consultar sus tickets y ver equipos asignados |

---

## Instrucciones de Ejecución

### 1. Requisitos Previos
- [.NET 10 SDK](https://dotnet.microsoft.com/)
- Servidor MySQL activo (Laragon, XAMPP o servicio local en el puerto `3306`).

### 2. Configuración de Base de Datos
Verificar la cadena de conexión en `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=it_manager_db;User=root;Password=;"
}
```

### 3. Compilación y Ejecución
Desde la raíz del proyecto web (`Integrador_Final_LabProgII/Integrador_Final_LabProgII`):
```bash
dotnet restore
dotnet build
dotnet run --launch-profile http
```

La aplicación quedará disponible en:
- Interfaz Web: **http://localhost:5062**
- Redirección automática de autenticación: `http://localhost:5062/Account/Login`
