using Application.Abstractions.Messaging;
using Application.Library.Backup.Shared;

namespace Application.Library.Backup.GetInfo;

public sealed record GetBackupInfoQuery : IQuery<BackupInfoResponse>;
