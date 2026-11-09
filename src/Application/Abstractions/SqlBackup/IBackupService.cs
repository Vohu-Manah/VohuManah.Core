using System.IO;

namespace Application.Abstractions.Backup;

public interface IBackupService
{
    
    Task RunBackupAsync(string fileName, CancellationToken cancellationToken);

    BackupProgress GetProgress();

    
    
    
    
    Task<bool> CancelAsync(CancellationToken cancellationToken);

    
    Task<BackupFileInfo> GetBackupInfoAsync(string fileName, CancellationToken cancellationToken);

    
    
    
    
    Task<BackupFileInfo> GetExistingBackupAsync(CancellationToken cancellationToken);

    
    Task<byte[]> ReadFileAsync(string fileName, bool deleteAfterRead, CancellationToken cancellationToken);

    
    
    
    
    
    Task<Stream> OpenReadAsync(string fileName, bool deleteAfterRead, CancellationToken cancellationToken);

    
    Task RestoreFromUploadAsync(Stream backupContent, string fileName, CancellationToken cancellationToken);

    
    Task RestoreDailyAsync(CancellationToken cancellationToken);

    
    
    
    
    
    Task CleanupExpiredOneTimeBackupAsync(TimeSpan maxAge, CancellationToken cancellationToken);
}