using Application.Abstractions.Messaging;

namespace Application.Library.Backup.Download;

public sealed record DownloadBackupQuery(string FileName) : IQuery<BackupDownloadFile>;
