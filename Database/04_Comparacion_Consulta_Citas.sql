
USE VetCareDB_Prueba;
GO

SET NOCOUNT ON;
SET STATISTICS IO ON;
SET STATISTICS TIME ON;

DECLARE @VeterinarioId INT = 1;
DECLARE @Inicio DATETIME2 = '2026-10-03T10:00:00';
DECLARE @Fin DATETIME2 = '2026-10-03T11:00:00';

-- CONSULTA A: aplica una función sobre la fecha
PRINT 'CONSULTA A - FILTRO NO OPTIMIZADO';

SELECT Id, VeterinarioId, FechaHora, Estado
FROM dbo.Citas
WHERE VeterinarioId = @VeterinarioId
  AND CAST(FechaHora AS DATE) = CAST(@Inicio AS DATE)
  AND FechaHora >= @Inicio
  AND FechaHora < @Fin;

-- CONSULTA B: filtra directamente por rango
PRINT 'CONSULTA B - FILTRO POR RANGO';

SELECT Id, VeterinarioId, FechaHora, Estado
FROM dbo.Citas
WHERE VeterinarioId = @VeterinarioId
  AND FechaHora >= @Inicio
  AND FechaHora < @Fin;

SET STATISTICS IO OFF;
SET STATISTICS TIME OFF;
GO
