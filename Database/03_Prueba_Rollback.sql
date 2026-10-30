
USE VetCareDB_Prueba;
GO

SET NOCOUNT ON;

-- Identificador único para esta prueba
DECLARE @Motivo NVARCHAR(250) =
    N'PRUEBA_ROLLBACK_' + CONVERT(NVARCHAR(36), NEWID());

DECLARE @Fecha DATETIME2 = DATEADD(DAY, 60, GETDATE());

-- Verificar registros antes de comenzar
SELECT 'ANTES' AS Momento, COUNT(*) AS Total
FROM dbo.Citas
WHERE Motivo = @Motivo;

BEGIN TRY
    BEGIN TRANSACTION;

    -- Registrar una cita temporal
    INSERT INTO dbo.Citas
        (MascotaId, VeterinarioId, FechaHora, Motivo, Estado)
    VALUES
        (1, 1, @Fecha, @Motivo, N'Programada');

    -- Comprobar que existe dentro de la transacción
    SELECT 'DURANTE' AS Momento, COUNT(*) AS Total
    FROM dbo.Citas
    WHERE Motivo = @Motivo;

    -- Simular un fallo para comprobar el rollback
    THROW 50001, 'Error simulado: se revierte la operación.', 1;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    SELECT ERROR_MESSAGE() AS MensajeError;
END CATCH;

-- Verificar que el registro no quedó guardado
SELECT 'DESPUES' AS Momento, COUNT(*) AS Total
FROM dbo.Citas
WHERE Motivo = @Motivo;
GO
