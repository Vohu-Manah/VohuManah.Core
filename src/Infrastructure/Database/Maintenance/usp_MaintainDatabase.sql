CREATE OR ALTER PROCEDURE dbo.usp_MaintainDatabase
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @dbName sysname = DB_NAME();
    DECLARE @logFileId int = (SELECT file_id FROM sys.database_files WHERE type = 2);
    DECLARE @dataFileId int = (SELECT file_id FROM sys.database_files WHERE type = 1);
    DECLARE @logSizeBefore bigint, @dataSizeBefore bigint;
    DECLARE @logSizeAfter bigint, @dataSizeAfter bigint;
    DECLARE @freedLog bigint, @freedData bigint;
    DECLARE @rebuilt int = 0;

    SELECT @logSizeBefore = size FROM sys.database_files WHERE file_id = @logFileId;
    SELECT @dataSizeBefore = size FROM sys.database_files WHERE file_id = @dataFileId;

    
    
    DECLARE @sqlMaint nvarchar(max) = N'';

    SELECT @sqlMaint = @sqlMaint + N'ALTER INDEX ' + QUOTENAME(i.name) + N' ON ' + QUOTENAME(s.name) + N' REBUILD;'
    FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED') ps
    JOIN sys.indexes i ON ps.object_id = i.object_id AND ps.index_id = i.index_id
    JOIN sys.tables s ON i.object_id = s.object_id
    WHERE ps.page_count >= 1000
      AND ps.avg_fragmentation_in_percent > 20
      AND i.name IS NOT NULL;

    IF LEN(@sqlMaint) > 0
    BEGIN
        EXEC sp_executesql @sqlMaint;
        SELECT @rebuilt = @@ROWCOUNT;
    END

    
    IF EXISTS (SELECT 1 FROM sys.databases WHERE name = @dbName AND recovery_model_desc = 'SIMPLE')
    BEGIN
        CHECKPOINT;
        BEGIN TRY
            DBCC SHRINKFILE (@logFileId, 64); 
        END TRY
        BEGIN CATCH
            
        END CATCH
    END

    
    BEGIN TRY
        DBCC SHRINKFILE (@dataFileId, 0); 
    END TRY
    BEGIN CATCH
    END CATCH

    SELECT @logSizeAfter = size FROM sys.database_files WHERE file_id = @logFileId;
    SELECT @dataSizeAfter = size FROM sys.database_files WHERE file_id = @dataFileId;
    SET @freedLog = @logSizeBefore - @logSizeAfter;
    SET @freedData = @dataSizeBefore - @dataSizeAfter;

    
    
    IF DB_NAME() = 'Library'
    BEGIN
        IF EXISTS (SELECT 1 FROM sys.database_files WHERE name = 'Library' AND is_percent_growth = 0)
            ALTER DATABASE [Library] MODIFY FILE (NAME = N'Library', FILEGROWTH = 10%);
        IF EXISTS (SELECT 1 FROM sys.database_files WHERE name = 'Library_log' AND is_percent_growth = 0)
            ALTER DATABASE [Library] MODIFY FILE (NAME = N'Library_log', FILEGROWTH = 64MB);
    END

    SELECT
        @rebuilt AS indexes_rebuilt,
        CAST(@freedLog / 128.0 AS DECIMAL(18,2)) AS freed_log_mb,
        CAST(@freedData / 128.0 AS DECIMAL(18,2)) AS freed_data_mb,
        CAST(@dataSizeAfter / 128.0 / 1024 AS DECIMAL(18,2)) AS data_size_gb,
        CAST((SELECT SUM(CAST(size AS BIGINT)) * 8.0 / 1024 / 1024 FROM sys.database_files) AS DECIMAL(18,2)) AS total_size_gb,
        (SELECT recovery_model_desc FROM sys.databases WHERE name = @dbName) AS recovery_model,
        (SELECT COUNT(*) FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED') WHERE page_count >= 1000 AND avg_fragmentation_in_percent > 20) AS remaining_fragmented;
END
