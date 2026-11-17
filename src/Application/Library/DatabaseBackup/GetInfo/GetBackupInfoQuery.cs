using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.DatabaseBackup.Shared;

namespace VohuManah.Application.Library.DatabaseBackup.GetInfo;

public sealed record GetBackupInfoQuery : IQuery<BackupInfoResponse>;
