# IT Manager - Plataforma Integral de Gestión de Activos, Mesa de Ayuda y Mantenimiento

Sistema web corporativo desarrollado en **ASP.NET Core (.NET 10)** con arquitectura limpia por capas y persistencia relacional en **MySQL** a través de **Entity Framework Core (Pomelo)**.

---

## 📌 Presentación y Dominio del Problema

**IT Manager** es una solución integral diseñada para departamentos de Tecnología de la Información corporativos. Resuelve la trazabilidad patrimonial de hardware, la administración del ciclo de vida de los incidentes reportados por los usuarios internos (Help Desk), la asignación de técnicos, la ejecución de órdenes de trabajo con descuento transaccional automático de repuestos del pañol y la exposición de servicios mediante Web API REST autenticada con JWT.

El proyecto cuenta con un calibre técnico y funcional equivalente al sistema de Inmobiliaria trabajado en clase, cumpliendo holgadamente todos los requerimientos mínimos de regularidad y promoción.

---

## 📋 Matriz de Evidencias de Requerimientos Mínimos

A continuación se detalla la matriz que certifica el cumplimiento del 100% de los requerimientos solicitados por la cátedra para la promoción de la asignatura:

| # | Requerimiento Mínimo | Estado | Enlace al Código / Controladores / Vistas | Evidencia / Mecanismo de Verificación |
|---|---|:---:|---|---|
| **1** | **Modelo de Datos y Persistencia:** $\ge 6$ tablas relacionadas, 1:N y N:M con entidad intermedia. | ✅ **Cumplido al 100%** | [`Data/AppDbContext.cs`](Integrador_Final_LabProgII/Data/AppDbContext.cs)<br>[`Models/`](Integrador_Final_LabProgII/Models/) | 10 tablas relacionadas. Relación N:M entre `Mantenimientos` y `Repuestos` materializada en la entidad intermedia `DetallesMantenimientos` con congelamiento de costos. Diagrama DER en [`docs/diagrama-er.drawio`](docs/diagrama-er.drawio) y diagrama de clases en [`docs/diagrama-clases (4).png`](docs/diagrama-clases%20(4).png). |
| **2** | **Seguridad, Roles y Avatar:** Login/Logout, RBAC (`ADMINISTRADOR`, `TECNICO`, `EMPLEADO`), prevención IDOR y **Gestión de Avatar personalizado**. | ✅ **Cumplido al 100%** | [`AccountController.cs#L125`](Integrador_Final_LabProgII/Controllers/AccountController.cs)<br>[`Views/Account/Profile.cshtml`](Integrador_Final_LabProgII/Views/Account/Profile.cshtml)<br>[`Views/Shared/_Layout.cshtml#L160`](Integrador_Final_LabProgII/Views/Shared/_Layout.cshtml) | Autenticación por Cookies con claims y expiración deslizante. Vista **"Mi Perfil"** (`/Account/Profile`) con subida y almacenamiento de avatar físico en `wwwroot/uploads/avatars/`, actualizando dinámicamente el badge del Navbar. Control IDOR en tickets y equipos según rol. |
| **3** | **Manejo de Archivos:** Procesamiento y almacenamiento en servidor adicional al avatar. | ✅ **Cumplido al 100%** | [`AdjuntosController.cs#L29`](Integrador_Final_LabProgII/Controllers/AdjuntosController.cs)<br>[`TicketsController.cs#L130`](Integrador_Final_LabProgII/Controllers/TicketsController.cs) | Subida física de imágenes y PDFs (`.jpg`, `.png`, `.pdf`) en `wwwroot/uploads/` con nombres normalizados mediante GUID y persistencia en tabla `Adjuntos` tanto en Tickets como en Mantenimientos. |
| **4** | **Módulo SPA / AJAX con Vue.js:** ABM completo reactivo resuelto vía AJAX/JSON sin recargar página. | ✅ **Cumplido al 100%** | [`CategoriasController.cs#L107`](Integrador_Final_LabProgII/Controllers/CategoriasController.cs)<br>[`Views/Categorias/Index.cshtml`](Integrador_Final_LabProgII/Views/Categorias/Index.cshtml) | Módulo **Categorías de Hardware**: SPA reactiva construida con **Vue.js 3**. Operaciones de **Alta, Baja, Modificación y Listado con filtrado en vivo** ejecutadas 100% vía `fetch` asíncrono y modales Bootstrap sin refresh de página. |
| **5** | **Paginado por Servidor (Server-Side):** Paginación en servidor con solicitud exclusiva de la página actual. | ✅ **Cumplido al 100%** | [`EquiposController.cs#L55`](Integrador_Final_LabProgII/Controllers/EquiposController.cs)<br>[`TicketsController.cs#L50`](Integrador_Final_LabProgII/Controllers/TicketsController.cs)<br>[`Repositories/Implementations/EquipoRepository.cs`](Integrador_Final_LabProgII/Repositories/Implementations/EquipoRepository.cs) | Consultas con `Skip()` y `Take()` ejecutadas directamente en MySQL (`page`, `pageSize`, `searchTerm`, filtros por estado y categoría). |
| **6** | **Búsqueda Asíncrona de Entidades Relacionadas:** Autocompletado AJAX en servidor sin `<select>` masivo. | ✅ **Cumplido al 100%** | [`EquiposController.cs#L212`](Integrador_Final_LabProgII/Controllers/EquiposController.cs)<br>[`Views/Tickets/Create.cshtml#L25`](Integrador_Final_LabProgII/Views/Tickets/Create.cshtml) | Al reportar un incidente (`/Tickets/Create`), la asociación del equipo se realiza mediante un input predictivo conectado al endpoint asíncrono `GET /Equipos/Search?term=...` con debounce, seleccionando el activo vía AJAX. |
| **7** | **Web API REST con JWT:** Endpoints asegurados con Bearer Token y colección de pruebas exportada. | ✅ **Cumplido al 100%** | [`Program.cs#L58`](Integrador_Final_LabProgII/Program.cs)<br>[`Controllers/Api/AuthApiController.cs`](Integrador_Final_LabProgII/Controllers/Api/AuthApiController.cs)<br>[`Controllers/Api/EquiposApiController.cs`](Integrador_Final_LabProgII/Controllers/Api/EquiposApiController.cs)<br>[`docs/ITManager_Postman_Collection.json`](docs/ITManager_Postman_Collection.json) | Endpoints `/api/auth/login` (emisión de JWT firmado con claims) y `/api/equipos` (GET paginado protegido con `[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]`). Colección de pruebas de Postman provista en el repositorio. |

---

## 🛠️ Tecnologías y Librerías

- **Framework:** .NET 10 (C# 13 / ASP.NET Core MVC)
- **Base de Datos:** MySQL 8.x / MariaDB a través de `Pomelo.EntityFrameworkCore.MySql`
- **Frontend & Reactividad:** Bootstrap 5.3 (Dark Theme) | Bootstrap Icons | **Vue.js 3 (SPA)** | Chart.js
- **Seguridad & Criptografía:** Doble esquema de autenticación (Cookies + JWT Bearer) | PBKDF2 Hasher (100.000 iteraciones SHA256 con salt criptográfico)

---

## 🔑 Credenciales de Acceso (Usuarios de Prueba)

El sistema inicializa automáticamente al arrancar o mediante el script SQL las siguientes cuentas para cada rol:

| Rol | Correo Electrónico | Contraseña | Alcance de Permisos |
| :--- | :--- | :--- | :--- |
| **Administrador** | `admin@itmanager.com` | `Admin123!*` | Control total del sistema, catálogos (Categorías Vue.js, Ubicaciones, Repuestos), gestión de usuarios y reseteo de claves. |
| **Técnico** | `tecnico@itmanager.com` | `Admin123!*` | Mesa de ayuda (Tickets), taller de reparaciones (Mantenimientos con consumo de stock) y reposición de pañol. |
| **Empleado** | `empleado@itmanager.com` | `Admin123!*` | Portal de autoservicio: apertura de incidentes con búsqueda asíncrona, seguimiento de tickets propios y perfil/avatar. |

---

## 📁 Entregables de Promoción Incluidos en el Repositorio

1. **Script de Base de Datos con Datos Semilla:**
   - Archivo: [`docs/database_init.sql`](docs/database_init.sql)
   - Contiene la creación de tablas, relaciones, claves foráneas, índices únicos y datos precargados para su ejecución directa en phpMyAdmin, DBeaver, MySQL Workbench o consola.
2. **Colección de Pruebas de Web API (Postman):**
   - Archivo: [`docs/ITManager_Postman_Collection.json`](docs/ITManager_Postman_Collection.json)
   - Pruebas configuradas para obtener token en `/api/auth/login` y consumir automáticamente `/api/equipos` inyectando el Bearer token en variables de entorno.
3. **Diagramas de Arquitectura:**
   - Diagrama de Entidad-Relación: [`docs/diagrama-er.drawio`](docs/diagrama-er.drawio)
   - Diagrama de Clases: [`docs/diagrama-clases (4).png`](docs/diagrama-clases%20(4).png)

---

## ⚙️ Instrucciones de Ejecución

### 1. Requisitos
- [.NET 10 SDK](https://dotnet.microsoft.com/)
- Motor MySQL / MariaDB (XAMPP, Laragon o servicio local en puerto `3306`).

### 2. Base de Datos
Podés importar el archivo [`docs/database_init.sql`](docs/database_init.sql) o simplemente iniciar la aplicación: el `DbInitializer` integrado creará las tablas y sembrará los datos automáticamente si la base está vacía.

Verificar en `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=it_manager_db;User=root;Password=;"
}
```

### 3. Compilación y Arranque
```bash
cd Integrador_Final_LabProgII
dotnet run --launch-profile http
```

La aplicación iniciará en:
👉 **http://localhost:5062** (Redirigiendo a `/Account/Login`)
