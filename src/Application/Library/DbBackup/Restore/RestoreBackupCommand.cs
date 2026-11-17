using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.DbBackup.Restore;

public sealed record RestoreBackupCommand(
    BackupRestoreSource Source,
    Stream? UploadedContent,
    string? UploadedFileName) : ICommand;
