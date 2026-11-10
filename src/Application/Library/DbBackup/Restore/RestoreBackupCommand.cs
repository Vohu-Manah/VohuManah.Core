using Application.Abstractions.Messaging;

namespace Application.Library.Backup.Restore;

public sealed record RestoreBackupCommand(
    BackupRestoreSource Source,
    Stream? UploadedContent,
    string? UploadedFileName) : ICommand;
