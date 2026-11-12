-- ====================================================================
-- IT MANAGER - SCRIPT DE BASE DE DATOS Y DATOS DE PRUEBA (SEED DUMP)
-- Laboratorio de Programación II - Proyecto Integrador Final MVC
-- Compatible con MySQL 8.x / MariaDB
-- ====================================================================

CREATE DATABASE IF NOT EXISTS `it_manager_db` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE `it_manager_db`;

SET FOREIGN_KEY_CHECKS = 0;

DROP TABLE IF EXISTS `Adjuntos`;
DROP TABLE IF EXISTS `ComentariosTickets`;
DROP TABLE IF EXISTS `Asignaciones`;
DROP TABLE IF EXISTS `DetallesMantenimientos`;
DROP TABLE IF EXISTS `Mantenimientos`;
DROP TABLE IF EXISTS `Repuestos`;
DROP TABLE IF EXISTS `Tickets`;
DROP TABLE IF EXISTS `Equipos`;
DROP TABLE IF EXISTS `Usuarios`;
DROP TABLE IF EXISTS `Categorias`;
DROP TABLE IF EXISTS `Ubicaciones`;

-- 1. Tabla Ubicaciones
CREATE TABLE `Ubicaciones` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Nombre` VARCHAR(100) NOT NULL,
    `Direccion` VARCHAR(255) NOT NULL,
    `PisoSector` VARCHAR(100) NULL,
    PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 2. Tabla Categorias
CREATE TABLE `Categorias` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Nombre` VARCHAR(100) NOT NULL,
    `Descripcion` VARCHAR(255) NULL,
    PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 3. Tabla Usuarios
CREATE TABLE `Usuarios` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Nombre` VARCHAR(100) NOT NULL,
    `Apellido` VARCHAR(100) NOT NULL,
    `Email` VARCHAR(150) NOT NULL,
    `PasswordHash` VARCHAR(255) NOT NULL,
    `Rol` VARCHAR(50) NOT NULL,
    `Activo` TINYINT(1) NOT NULL DEFAULT 1,
    `AvatarUrl` VARCHAR(255) NULL,
    `UbicacionId` INT NOT NULL,
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `UpdatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `UX_Usuarios_Email` (`Email`),
    CONSTRAINT `FK_Usuarios_Ubicaciones` FOREIGN KEY (`UbicacionId`) REFERENCES `Ubicaciones` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 4. Tabla Equipos
CREATE TABLE `Equipos` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Codigo` VARCHAR(50) NOT NULL,
    `NumeroSerie` VARCHAR(100) NOT NULL,
    `Marca` VARCHAR(100) NOT NULL,
    `Modelo` VARCHAR(100) NOT NULL,
    `Especificaciones` TEXT NULL,
    `FechaAdquisicion` DATETIME NOT NULL,
    `Estado` VARCHAR(50) NOT NULL,
    `CategoriaId` INT NOT NULL,
    `UbicacionId` INT NOT NULL,
    `UsuarioCustodioId` INT NULL,
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `UpdatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `UX_Equipos_Codigo` (`Codigo`),
    UNIQUE KEY `UX_Equipos_NumeroSerie` (`NumeroSerie`),
    CONSTRAINT `FK_Equipos_Categorias` FOREIGN KEY (`CategoriaId`) REFERENCES `Categorias` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Equipos_Ubicaciones` FOREIGN KEY (`UbicacionId`) REFERENCES `Ubicaciones` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Equipos_Usuarios` FOREIGN KEY (`UsuarioCustodioId`) REFERENCES `Usuarios` (`Id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 5. Tabla Repuestos
CREATE TABLE `Repuestos` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Codigo` VARCHAR(50) NOT NULL,
    `Nombre` VARCHAR(150) NOT NULL,
    `Descripcion` VARCHAR(255) NULL,
    `StockActual` INT NOT NULL DEFAULT 0,
    `StockMinimo` INT NOT NULL DEFAULT 5,
    `PrecioUnitario` DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `UpdatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `UX_Repuestos_Codigo` (`Codigo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 6. Tabla Tickets
CREATE TABLE `Tickets` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Codigo` VARCHAR(50) NOT NULL,
    `Titulo` VARCHAR(200) NOT NULL,
    `Descripcion` TEXT NOT NULL,
    `Prioridad` VARCHAR(50) NOT NULL,
    `Estado` VARCHAR(50) NOT NULL,
    `FechaApertura` DATETIME NOT NULL,
    `FechaCierre` DATETIME NULL,
    `EquipoId` INT NOT NULL,
    `UsuarioSolicitanteId` INT NOT NULL,
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `UpdatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `UX_Tickets_Codigo` (`Codigo`),
    CONSTRAINT `FK_Tickets_Equipos` FOREIGN KEY (`EquipoId`) REFERENCES `Equipos` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Tickets_Usuarios` FOREIGN KEY (`UsuarioSolicitanteId`) REFERENCES `Usuarios` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 7. Tabla Asignaciones (Técnicos a Tickets)
CREATE TABLE `Asignaciones` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `TicketId` INT NOT NULL,
    `UsuarioTecnicoId` INT NOT NULL,
    `FechaAsignacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `FechaFin` DATETIME NULL,
    `Activo` TINYINT(1) NOT NULL DEFAULT 1,
    `Notas` VARCHAR(255) NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Asignaciones_Tickets` FOREIGN KEY (`TicketId`) REFERENCES `Tickets` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Asignaciones_Usuarios` FOREIGN KEY (`UsuarioTecnicoId`) REFERENCES `Usuarios` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 8. Tabla Mantenimientos
CREATE TABLE `Mantenimientos` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Tipo` VARCHAR(50) NOT NULL,
    `FechaInicio` DATETIME NOT NULL,
    `FechaFin` DATETIME NULL,
    `Diagnostico` TEXT NOT NULL,
    `Solucion` TEXT NULL,
    `HorasHombre` DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    `CostoTotal` DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    `EquipoId` INT NOT NULL,
    `TicketId` INT NULL,
    `UsuarioTecnicoId` INT NOT NULL,
    `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `UpdatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Mantenimientos_Equipos` FOREIGN KEY (`EquipoId`) REFERENCES `Equipos` (`Id`) ON DELETE RESTRICT,
    CONSTRAINT `FK_Mantenimientos_Tickets` FOREIGN KEY (`TicketId`) REFERENCES `Tickets` (`Id`) ON DELETE SET NULL,
    CONSTRAINT `FK_Mantenimientos_Usuarios` FOREIGN KEY (`UsuarioTecnicoId`) REFERENCES `Usuarios` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 9. Tabla DetallesMantenimientos (Relación N:M Mantenimiento <-> Repuestos con congelamiento de costo)
CREATE TABLE `DetallesMantenimientos` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `MantenimientoId` INT NOT NULL,
    `RepuestoId` INT NOT NULL,
    `Cantidad` INT NOT NULL,
    `CostoHistorico` DECIMAL(18,2) NOT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Detalles_Mantenimientos` FOREIGN KEY (`MantenimientoId`) REFERENCES `Mantenimientos` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Detalles_Repuestos` FOREIGN KEY (`RepuestoId`) REFERENCES `Repuestos` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 10. Tabla ComentariosTickets
CREATE TABLE `ComentariosTickets` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `TicketId` INT NOT NULL,
    `UsuarioId` INT NOT NULL,
    `Mensaje` TEXT NOT NULL,
    `Fecha` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Comentarios_Tickets` FOREIGN KEY (`TicketId`) REFERENCES `Tickets` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Comentarios_Usuarios` FOREIGN KEY (`UsuarioId`) REFERENCES `Usuarios` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 11. Tabla Adjuntos (Físico en wwwroot/uploads/)
CREATE TABLE `Adjuntos` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `NombreArchivo` VARCHAR(255) NOT NULL,
    `Ruta` VARCHAR(255) NOT NULL,
    `MimeType` VARCHAR(100) NOT NULL,
    `TamanoBytes` BIGINT NOT NULL,
    `FechaSubida` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `TicketId` INT NULL,
    `MantenimientoId` INT NULL,
    PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Adjuntos_Tickets` FOREIGN KEY (`TicketId`) REFERENCES `Tickets` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_Adjuntos_Mantenimientos` FOREIGN KEY (`MantenimientoId`) REFERENCES `Mantenimientos` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

SET FOREIGN_KEY_CHECKS = 1;

-- ====================================================================
-- CARGA DE DATOS DE PRUEBA (SEED DATA)
-- ====================================================================

-- Sedes
INSERT INTO `Ubicaciones` (`Id`, `Nombre`, `Direccion`, `PisoSector`) VALUES
(1, 'Casa Central', 'Av. Corrientes 1234, CABA', 'Piso 4 - Data Center & HelpDesk'),
(2, 'Sucursal Norte', 'Av. Maipú 2500, Olivos', 'Planta Baja - Operaciones'),
(3, 'Centro de Distribución', 'Ruta 9 Km 35, Tigre', 'Galpón 2 - Logística');

-- Categorías
INSERT INTO `Categorias` (`Id`, `Nombre`, `Descripcion`) VALUES
(1, 'Notebooks', 'Equipos portátiles corporativos'),
(2, 'Servidores', 'Hardware crítico de infraestructura de cómputo'),
(3, 'Workstations', 'Estaciones de trabajo de alto rendimiento'),
(4, 'Impresoras', 'Equipos multifunción y térmicos'),
(5, 'Networking', 'Switches, routers y access points');

-- Usuarios (Clave de todos: Admin123!*)
-- Hash PBKDF2: 100000.yJtN51vGfJ32c9rT9M2q1A==.p+c4YF1h0Qh+p+T0k+lE2bX7O0jY/Q7c3c+w9K4c+dM=
INSERT INTO `Usuarios` (`Id`, `Nombre`, `Apellido`, `Email`, `PasswordHash`, `Rol`, `Activo`, `AvatarUrl`, `UbicacionId`) VALUES
(1, 'Administrador', 'General', 'admin@itmanager.com', '100000.yJtN51vGfJ32c9rT9M2q1A==.p+c4YF1h0Qh+p+T0k+lE2bX7O0jY/Q7c3c+w9K4c+dM=', 'ADMINISTRADOR', 1, '/img/avatars/default.png', 1),
(2, 'Martín', 'Gómez', 'tecnico@itmanager.com', '100000.yJtN51vGfJ32c9rT9M2q1A==.p+c4YF1h0Qh+p+T0k+lE2bX7O0jY/Q7c3c+w9K4c+dM=', 'TECNICO', 1, '/img/avatars/default.png', 1),
(3, 'Laura', 'Sánchez', 'empleado@itmanager.com', '100000.yJtN51vGfJ32c9rT9M2q1A==.p+c4YF1h0Qh+p+T0k+lE2bX7O0jY/Q7c3c+w9K4c+dM=', 'EMPLEADO', 1, '/img/avatars/default.png', 2);

-- Equipos
INSERT INTO `Equipos` (`Id`, `Codigo`, `NumeroSerie`, `Marca`, `Modelo`, `Especificaciones`, `FechaAdquisicion`, `Estado`, `CategoriaId`, `UbicacionId`, `UsuarioCustodioId`) VALUES
(1, 'NB-CORP-001', 'SN-LEN-88912', 'Lenovo', 'ThinkPad T14 Gen 4', 'Core i7 1370P, 32GB RAM, 1TB NVMe', '2024-03-15', 'OPERATIVO', 1, 2, 3),
(2, 'PC-LAB-042', 'SN-DELL-55210', 'Dell', 'OptiPlex 7090', 'Core i5 11500, 16GB RAM, 512GB SSD', '2023-08-10', 'EN_REPARACION', 3, 1, NULL),
(3, 'SRV-DATA-001', 'SN-HPE-99001', 'HP', 'ProLiant DL380 Gen10', '2x Xeon Silver 4210R, 128GB ECC RAM, 8x 2.4TB SAS', '2022-11-05', 'OPERATIVO', 2, 1, 1);

-- Pañol de Repuestos
INSERT INTO `Repuestos` (`Id`, `Codigo`, `Nombre`, `Descripcion`, `StockActual`, `StockMinimo`, `PrecioUnitario`) VALUES
(1, 'REP-SSD-1TB', 'Disco SSD NVMe 1TB Kingston', 'M.2 PCIe 4.0 NV2', 8, 3, 75.50),
(2, 'REP-RAM-16GB', 'Memoria RAM DDR4 16GB Kingston', '3200MHz SODIMM para Notebook', 4, 5, 45.00),
(3, 'REP-FAN-CPU', 'Cooler Ventilador CPU Original', 'Disipador térmico para Dell OptiPlex', 2, 4, 18.20),
(4, 'REP-FUENTE-65W', 'Cargador Original 65W USB-C', 'Compatible ThinkPad y Dell Latitude', 12, 4, 38.00);

-- Tickets de Soporte
INSERT INTO `Tickets` (`Id`, `Codigo`, `Titulo`, `Descripcion`, `Prioridad`, `Estado`, `FechaApertura`, `FechaCierre`, `EquipoId`, `UsuarioSolicitanteId`) VALUES
(1, 'TK-2026-INIT', 'Pantallazo azul recurrente al encender', 'El equipo emite un error DRIVER_IRQL_NOT_LESS_OR_EQUAL al cargar Windows.', 'CRITICA', 'EN_PROCESO', '2026-11-10 09:30:00', NULL, 2, 3);

-- Asignación Técnica
INSERT INTO `Asignaciones` (`Id`, `TicketId`, `UsuarioTecnicoId`, `FechaAsignacion`, `FechaFin`, `Activo`, `Notas`) VALUES
(1, 1, 2, '2026-11-10 10:00:00', NULL, 1, 'Revisión en taller central por el técnico Gómez.');

-- Comentarios
INSERT INTO `ComentariosTickets` (`Id`, `TicketId`, `UsuarioId`, `Mensaje`, `Fecha`) VALUES
(1, 1, 3, 'El fallo empezó después de la última actualización de Windows.', '2026-11-10 09:35:00'),
(2, 1, 2, 'Recibido en banco de prueba. Procedo a chequear integridad de memoria y temperaturas.', '2026-11-10 10:15:00');
