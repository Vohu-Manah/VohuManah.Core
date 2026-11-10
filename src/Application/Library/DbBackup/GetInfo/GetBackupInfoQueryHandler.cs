using Application.Abstractions.Backup;
using Application.Abstractions.Messaging;
using Application.Library.Backup.Shared;
using SharedKernel;

namespace Application.Library.Backup.GetInfo;

internal sealed class GetBackupInfoQueryHandler(
    IBackupService backupService) : IQueryHandler<GetBackupInfoQuery, BackupInfoResponse>
{
    public async Task<Result<BackupInfoResponse>> Handle(GetBackupInfoQuery query, CancellationToken cancellationToken)
    {
        BackupFileInfo info = await backupService
            .GetExistingBackupAsync(cancellationToken)
            .ConfigureAwait(false);

        BackupProgress progress = backupService.GetProgress();

        return Result.Success(new BackupInfoResponse(
            info.Exists,
            info.FileName,
            info.TakenAt,
            info.TakenTime,
            info.SizeBytes,
            progress.IsRunning,
            progress.Percent,
            progress.Stage,
            progress.Error,
            progress.Log));
    }
}