using Application.Abstractions.Backup;
using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Library.Backup.Download;

internal sealed class DownloadBackupQueryHandler(
    IBackupService backupService) : IQueryHandler<DownloadBackupQuery, BackupDownloadFile>
{
    public async Task<Result<BackupDownloadFile>> Handle(DownloadBackupQuery query, CancellationToken cancellationToken)
    {
        BackupFileInfo info = await backupService.GetBackupInfoAsync(query.FileName, cancellationToken)
            .ConfigureAwait(false);

        if (!info.Exists)
        {
            return Result.Failure<BackupDownloadFile>(Error.NotFound(
                "Backup.NotFound",
                "فایل بکاپ موجود نیست؛ ابتدا بکاپ بگیرید"));
        }

        
        
        bool isOneTime = query.FileName == Application.Library.Backup.Shared.BackupFileNames.OneTime;
        Stream stream = await backupService
            .OpenReadAsync(query.FileName, deleteAfterRead: isOneTime, cancellationToken)
            .ConfigureAwait(false);

        return Result.Success(new BackupDownloadFile(stream, query.FileName, info.SizeBytes));
    }
}
