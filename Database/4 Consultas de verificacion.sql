USE [VetCareDB_Prueba];
GO

SELECT COUNT(*) AS TotalTablas
FROM sys.tables;

SELECT 'Clientes' AS Tabla, COUNT(*) AS Registros FROM dbo.Clientes
UNION ALL
SELECT 'Mascotas', COUNT(*) FROM dbo.Mascotas
UNION ALL
SELECT 'Veterinarios', COUNT(*) FROM dbo.Veterinarios
UNION ALL
SELECT 'Citas', COUNT(*) FROM dbo.Citas
UNION ALL
SELECT 'Consultas', COUNT(*) FROM dbo.Consultas
UNION ALL
SELECT 'Tratamientos', COUNT(*) FROM dbo.Tratamientos
UNION ALL
SELECT 'Usuarios', COUNT(*) FROM dbo.AspNetUsers
UNION ALL
SELECT 'Roles', COUNT(*) FROM dbo.AspNetRoles;

SELECT
    m.Id AS MascotaId,
    m.Nombre AS Mascota,
    c.Nombres,
    c.Apellidos
FROM dbo.Mascotas AS m
INNER JOIN dbo.Clientes AS c ON c.Id = m.ClienteId;

SELECT
    co.Id AS ConsultaId,
    m.Nombre AS Mascota,
    v.Nombre AS Veterinario,
    co.Fecha,
    co.Diagnostico,
    t.Id AS TratamientoId,
    t.Descripcion AS Tratamiento
FROM dbo.Consultas AS co
INNER JOIN dbo.Mascotas AS m ON m.Id = co.MascotaId
INNER JOIN dbo.Veterinarios AS v ON v.Id = co.VeterinarioId
LEFT JOIN dbo.Tratamientos AS t ON t.ConsultaId = co.Id;

SELECT
    u.Email,
    r.Name AS Rol
FROM dbo.AspNetUsers AS u
INNER JOIN dbo.AspNetUserRoles AS ur ON ur.UserId = u.Id
INNER JOIN dbo.AspNetRoles AS r ON r.Id = ur.RoleId;

DBCC CHECKCONSTRAINTS WITH ALL_CONSTRAINTS;
GO