using VohuManah.Application.Abstractions.DatabaseBackup;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.DatabaseBackup.Shared;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.DatabaseBackup.GetInfo;

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