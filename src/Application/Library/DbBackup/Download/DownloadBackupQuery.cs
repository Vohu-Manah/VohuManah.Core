using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.DbBackup.Download;

public sealed record DownloadBackupQuery(string FileName) : IQuery<BackupDownloadFile>;
