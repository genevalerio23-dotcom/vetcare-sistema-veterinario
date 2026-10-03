# VetCare — Sistema de gestión veterinaria

## Avance del II Parcial

VetCare es una aplicación web para registrar propietarios y mascotas,
agendar citas, registrar consultas y tratamientos, y consultar el
historial clínico de cada mascota.

Este avance implementa una arquitectura organizada en cuatro capas:
Presentación, Aplicación, Dominio e Infraestructura.

## Tecnologías

- C# y .NET 8.
- ASP.NET Core MVC con vistas Razor.
- Entity Framework Core 8.
- SQL Server Express.
- ASP.NET Core Identity para autenticación y roles.
- Bootstrap para la interfaz.
- Visual Studio 2022.

## Organización del proyecto

| Proyecto o carpeta | Responsabilidad |
|---|---|
| VetCare.Web | Controladores, vistas, formularios y configuración de la aplicación. |
| VetCare.Application | Servicios que coordinan los casos de uso, interfaces y DTO. |
| VetCare.Domain | Entidades, reglas de negocio y contratos de repositorios. |
| VetCare.Infrastructure | Persistencia con Entity Framework Core, repositorios y migraciones. |
| Database | Scripts para reconstruir y verificar la base de datos. |
| docs | Documentación y evidencias del avance. |

La aplicación registra las dependencias en `VetCare.Web/Program.cs`.

Aplicación depende de Dominio. Infraestructura implementa los contratos
definidos en Dominio. Presentación utiliza los servicios de Aplicación
y registra las implementaciones de Infraestructura.

## Casos de uso implementados

1. Registrar cliente.
2. Registrar mascota y asociarla a un cliente.
3. Agendar cita con mascota y veterinario.
4. Registrar consulta clínica, con cita opcional.
5. Registrar tratamiento asociado a una consulta.
6. Consultar el historial clínico de una mascota.

La aplicación también permite iniciar y cerrar sesión y restringe
los módulos según el rol del usuario.

## Reglas de negocio

- Los datos obligatorios y las longitudes se validan antes de guardar.
- El peso de una mascota debe ser positivo, estar dentro del rango
  permitido y tener como máximo dos decimales.
- Una mascota debe registrarse con un cliente existente y activo.
- Las citas deben programarse para una fecha y hora futuras.
- Las citas tienen una duración de 30 minutos.
- No se permiten citas que se superpongan para la misma mascota
  o el mismo veterinario.
- Las consultas no pueden tener una fecha futura.
- Una consulta vinculada a una cita debe coincidir con su mascota
  y veterinario.
- Solo se puede atender una cita programada.
- Una cita no puede tener más de una consulta.
- Al registrar una consulta vinculada a una cita, esta se marca
  como atendida.
- Los tratamientos requieren una consulta existente.
- La fecha inicial de un tratamiento no puede ser anterior
  a la fecha de su consulta.
- La fecha final de un tratamiento no puede ser anterior
  a su fecha inicial.

Las reglas se distribuyen entre las entidades de Dominio y los
servicios de Aplicación. Infraestructura refuerza la integridad
mediante restricciones, índices y transacciones.

## Servicios y contratos

Los servicios de Aplicación incluyen:

- ClienteService e IClienteService.
- MascotaService e IMascotaService.
- CitaService e ICitaService.
- ConsultaService e IConsultaService.
- TratamientoService e ITratamientoService.
- HistorialService e IHistorialService.
- CatalogoService e ICatalogoService.

Los contratos de persistencia incluyen:

- IClienteRepository.
- IMascotaRepository.
- IVeterinarioRepository.
- ICitaRepository.
- IConsultaRepository.
- ITratamientoRepository.

Los DTO de entrada transportan los datos de registro. Los DTO de
salida permiten presentar las opciones de selección y el historial
sin entregar directamente las entidades a las vistas.

## Principios de diseño

### SRP — Responsabilidad única

Los controladores atienden las solicitudes web, los servicios
coordinan las operaciones, las entidades validan sus datos y los
repositorios realizan la persistencia.

### DIP — Inversión de dependencias

Los servicios reciben interfaces de repositorios por constructor.
Las implementaciones concretas se registran mediante inyección
de dependencias en `Program.cs`.

## Permisos

| Rol | Módulos permitidos |
|---|---|
| Administrador | Clientes, Mascotas, Citas, Consultas, Tratamientos e Historial. |
| Recepcionista | Clientes, Mascotas y Citas. |
| Veterinario | Consultas, Tratamientos e Historial. |

La autorización se aplica en los controladores mediante
`[Authorize(Roles = "...")]`. El menú y las tarjetas del inicio
también se adaptan al rol.

## Requisitos para ejecutar

- Visual Studio 2022 con soporte para proyectos .NET 8 y desarrollo web.
- SDK de .NET 8.
- SQL Server 2019 o posterior. El entorno de desarrollo utiliza
  SQL Server Express.
- SQL Server Management Studio para ejecutar los scripts.
- Acceso a NuGet para restaurar las dependencias.

## Instalación mediante el script SQL

### 1. Crear la base de datos

En SQL Server Management Studio, conecta con tu instancia y ejecuta
completo el archivo:

```text
Database/VetCareDB_Prueba.sql
```

El script crea `VetCareDB_Prueba`, sus tablas, índices, restricciones
y datos de prueba, incluyendo usuarios y roles.

La base `VetCareDB_Prueba` debe estar ausente antes de ejecutar
el script. No vuelvas a ejecutar el archivo sobre una base
que ya contiene las tablas.

El script fue preparado sin rutas físicas específicas para los
archivos de SQL Server.

### 2. Configurar la conexión

Abre `VetCare.Web/appsettings.json`.

Para usar la base reconstruida, configura:

```json
{
  "ConnectionStrings": {
    "VetCareDb": "Server=localhost\\SQLEXPRESS;Database=VetCareDB_Prueba;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

Si tu instancia tiene otro nombre, cambia
`localhost\\SQLEXPRESS` por el nombre correspondiente.

Esta conexión usa autenticación de Windows. El usuario que ejecuta
la aplicación debe tener permisos sobre la base de datos.

`TrustServerCertificate=True` corresponde a la configuración
local de desarrollo.

El proyecto original utiliza `VetCareDB`. No necesitas cambiar
su conexión si vas a seguir trabajando con esa base.

### 3. Abrir y ejecutar

1. Abre `VetCare.sln` en Visual Studio.
2. Restaura los paquetes NuGet.
3. Selecciona `VetCare.Web` como proyecto de inicio.
4. Compila la solución.
5. Selecciona el perfil HTTPS y ejecuta con `Ctrl + F5`.
6. Utiliza la dirección HTTPS que abra Visual Studio.
7. Ingresa mediante el botón “Iniciar sesión”.

La cookie de autenticación requiere HTTPS.

## Usuarios de demostración

| Rol | Correo | Contraseña |
|---|---|---|
| Administrador | admin@vetcare.local | VetCareAdmin2026! |
| Recepcionista | recepcion@vetcare.local | VetCareRecepcion2026! |
| Veterinario | veterinario@vetcare.local | VetCareVeterinario2026! |

Estas cuentas y contraseñas son exclusivamente de demostración.

Identity almacena hashes de las contraseñas en la base de datos.

En el entorno `Development`, `IdentitySeeder` crea las cuentas
y los roles que falten. No restablece las contraseñas de usuarios
existentes.

## Persistencia

`VetCareDbContext` administra:

- Clientes.
- Mascotas.
- Veterinarios.
- Citas.
- Consultas.
- Tratamientos.

`VetCareIdentityDbContext` administra las tablas de Identity.

Ambos contextos utilizan la misma base de datos y tienen historiales
de migraciones separados:

- `__EFMigrationsHistory`.
- `__IdentityMigrationsHistory`.

Los repositorios se encuentran en Infraestructura. Los controladores
no contienen instrucciones SQL.

## Manejo de errores

- Los formularios muestran las validaciones de entrada.
- Las reglas de negocio rechazadas se presentan como mensajes
  comprensibles.
- Los controladores de registro capturan y registran los errores
  inesperados y muestran un mensaje general al usuario.
- Los accesos sin sesión se redirigen al inicio de sesión.
- Los accesos sin el rol requerido muestran “Acceso denegado”.
- Identity aplica bloqueo temporal después de cinco intentos fallidos
  de contraseña para una cuenta existente.

## Verificación de la base reconstruida

Ejecuta en SQL Server Management Studio:

```sql
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

DBCC CHECKCONSTRAINTS WITH ALL_CONSTRAINTS;
GO
```

El script entregado contiene 15 tablas y los siguientes registros:

| Tabla | Registros |
|---|---:|
| Clientes | 2 |
| Mascotas | 2 |
| Veterinarios | 2 |
| Citas | 1 |
| Consultas | 1 |
| Tratamientos | 1 |
| Usuarios | 3 |
| Roles | 3 |

Estos conteos corresponden al script exportado. Pueden aumentar
después de registrar nuevos datos.

La reconstrucción se comprobó en una base separada. La ejecución
de `DBCC CHECKCONSTRAINTS` no mostró incumplimientos.

## Pruebas funcionales

Las pruebas manuales del avance abarcan:

- Registro correcto y persistencia de los casos de uso.
- Rechazo de campos obligatorios vacíos.
- Rechazo de citas con fecha pasada.
- Rechazo de conflictos de horarios.
- Rechazo de fechas de tratamiento inconsistentes.
- Consulta del historial y sus tratamientos.
- Inicio y cierre de sesión.
- Restricción de acceso por rol.

Las capturas y los resultados obtenidos se organizan en
`docs/evidencias` y deben incorporarse al PDF del avance.

No se incluye una suite de pruebas automatizadas en esta versión.

## Alcance del avance

Esta versión corresponde al Hito II. Implementa los casos de uso
descritos, pero todavía no constituye el producto final.

No incluye módulos de pagos, inventario de medicamentos ni
administración de usuarios desde la interfaz.

## Documentación y repositorio

El PDF consolidado debe incluir la arquitectura actualizada,
los diagramas de capas, secuencia y vista lógica, las evidencias,
las correcciones del primer parcial y la URL del repositorio.

La URL, la rama principal y los aportes de los integrantes deben
corresponder al repositorio real utilizado para la entrega.