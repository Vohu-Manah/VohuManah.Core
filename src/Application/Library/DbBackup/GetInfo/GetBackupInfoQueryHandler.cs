using VohuManah.Application.Abstractions.SqlBackup;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.DbBackup.Shared;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.DbBackup.GetInfo;

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