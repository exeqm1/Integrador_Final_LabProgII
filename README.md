# IT Manager

Sistema web MVC para gestión de inventario informático, tickets de soporte y órdenes de mantenimiento con consumo de repuestos.

Desarrollado en **.NET 10** (C#) con **MySQL** y **Entity Framework Core**.

---

## Requerimientos Técnicos (Matriz de Evidencias)

| Requerimiento | Estado | Archivos / Controladores | Verificación |
|---|:---:|---|---|
| **1. Modelo de datos ($\ge 6$ tablas, 1:N y N:M)** | Cumplido | [`Data/AppDbContext.cs`](Integrador_Final_LabProgII/Data/AppDbContext.cs)<br>[`Models/`](Integrador_Final_LabProgII/Models/) | 10 entidades. N:M entre `Mantenimientos` y `Repuestos` con `DetallesMantenimientos`. DER en [`docs/diagrama-er.drawio`](docs/diagrama-er.drawio). |
| **2. Login, roles y avatar personalizado** | Cumplido | [`AccountController.cs`](Integrador_Final_LabProgII/Controllers/AccountController.cs)<br>[`Views/Account/Profile.cshtml`](Integrador_Final_LabProgII/Views/Account/Profile.cshtml) | Roles: `ADMINISTRADOR`, `TECNICO`, `EMPLEADO`. Protección IDOR. Perfil con subida de avatar en `wwwroot/uploads/avatars/`. |
| **3. Manejo de archivos** | Cumplido | [`AdjuntosController.cs`](Integrador_Final_LabProgII/Controllers/AdjuntosController.cs)<br>[`TicketsController.cs`](Integrador_Final_LabProgII/Controllers/TicketsController.cs) | Carga física de comprobantes y fotos en `wwwroot/uploads/` (PDF, PNG, JPG con GUID). |
| **4. ABM reactivo con Vue.js** | Cumplido | [`CategoriasController.cs`](Integrador_Final_LabProgII/Controllers/CategoriasController.cs)<br>[`Views/Categorias/Index.cshtml`](Integrador_Final_LabProgII/Views/Categorias/Index.cshtml) | SPA en Vue.js 3 para Categorías: listado, alta, edición y baja por AJAX/JSON sin recargar la página. |
| **5. Paginado por servidor** | Cumplido | [`EquiposController.cs`](Integrador_Final_LabProgII/Controllers/EquiposController.cs)<br>[`TicketsController.cs`](Integrador_Final_LabProgII/Controllers/TicketsController.cs) | Paginación en backend con `Skip` y `Take` (`GetPagedAsync`). |
| **6. Búsqueda asíncrona de entidades** | Cumplido | [`EquiposController.cs#L212`](Integrador_Final_LabProgII/Controllers/EquiposController.cs)<br>[`Views/Tickets/Create.cshtml`](Integrador_Final_LabProgII/Views/Tickets/Create.cshtml) | Búsqueda asíncrona vía AJAX (`/Equipos/Search?term=...`) al asociar un equipo a un ticket, sin select masivo. |
| **7. Web API REST con JWT** | Cumplido | [`Controllers/Api/`](Integrador_Final_LabProgII/Controllers/Api/)<br>[`docs/ITManager_Postman_Collection.json`](docs/ITManager_Postman_Collection.json) | Endpoints `/api/auth/login` y `/api/equipos` con Bearer token. Colección Postman incluida. |

---

## Usuarios de Prueba

| Rol | Email | Password |
|---|---|---|
| Administrador | `admin@itmanager.com` | `Admin123!*` |
| Técnico | `tecnico@itmanager.com` | `Admin123!*` |
| Empleado | `empleado@itmanager.com` | `Admin123!*` |

---

## Ejecución Local

1. Configurar MySQL en `Integrador_Final_LabProgII/appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=it_manager_db;User=root;Password=;"
}
```

2. Ejecutar:
```bash
cd Integrador_Final_LabProgII/Integrador_Final_LabProgII
dotnet run --launch-profile http
```

URL local: **http://localhost:5062**

---

## Archivos de Entrega en `docs/`
- Script SQL con datos: [`docs/database_init.sql`](docs/database_init.sql)
- Colección de Postman: [`docs/ITManager_Postman_Collection.json`](docs/ITManager_Postman_Collection.json)
- Diagramas: [`docs/diagrama-er.drawio`](docs/diagrama-er.drawio) y [`docs/diagrama-clases (4).png`](docs/diagrama-clases%20(4).png)
