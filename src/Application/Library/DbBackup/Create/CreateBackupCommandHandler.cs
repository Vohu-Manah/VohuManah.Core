using VohuManah.Application.Abstractions.SqlBackup;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.DbBackup.Shared;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.DbBackup.Create;

internal sealed class CreateBackupCommandHandler(
    IBackupService backupService) : ICommandHandler<CreateBackupCommand>
{
    public async Task<Result> Handle(CreateBackupCommand command, CancellationToken cancellationToken)
    {
        BackupProgress current = backupService.GetProgress();
        if (current.IsRunning)
        {
            return Result.Failure(Error.Conflict("Backup.AlreadyRunning", "یک بکاپ در حال اجراست؛ لطفاً صبر کنید"));
        }

        await backupService.RunBackupAsync(BackupFileNames.OneTime, cancellationToken).ConfigureAwait(false);

        return Result.Success();
    }
}
