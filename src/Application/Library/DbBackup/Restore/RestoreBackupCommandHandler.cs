using Application.Abstractions.Backup;
using Application.Abstractions.Messaging;
using Application.Library.Backup.Shared;
using SharedKernel;

namespace Application.Library.Backup.Restore;

internal sealed class RestoreBackupCommandHandler(
    IBackupService backupService) : ICommandHandler<RestoreBackupCommand>
{
    public async Task<Result> Handle(RestoreBackupCommand command, CancellationToken cancellationToken)
    {
        BackupProgress current = backupService.GetProgress();
        if (current.IsRunning)
        {
            return Result.Failure(Error.Conflict("Backup.Busy", "عملیات بکاپ/بازگردانی دیگری در حال اجراست"));
        }

        if (command.Source == BackupRestoreSource.Uploaded)
        {
            if (command.UploadedContent is null || string.IsNullOrWhiteSpace(command.UploadedFileName))
            {
                return Result.Failure(Error.Validation("Backup.NoFile", "فایل بکاپ ارسال نشده است"));
            }

            await backupService
                .RestoreFromUploadAsync(command.UploadedContent, command.UploadedFileName, cancellationToken)
                .ConfigureAwait(false);

            return Result.Success();
        }

        BackupFileInfo daily = await backupService
            .GetBackupInfoAsync(BackupFileNames.Daily, cancellationToken)
            .ConfigureAwait(false);

        if (!daily.Exists)
        {
            return Result.Failure(Error.NotFound("Backup.DailyNotFound", "فایل بکاپ روزانه موجود نیست"));
        }

        await backupService.RestoreDailyAsync(cancellationToken).ConfigureAwait(false);

        return Result.Success();
    }
}
