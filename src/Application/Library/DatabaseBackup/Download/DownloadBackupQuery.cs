using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.DatabaseBackup.Download;

public sealed record DownloadBackupQuery(string FileName) : IQuery<BackupDownloadFile>;
