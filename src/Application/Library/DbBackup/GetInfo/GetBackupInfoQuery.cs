using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.DbBackup.Shared;

namespace VohuManah.Application.Library.DbBackup.GetInfo;

public sealed record GetBackupInfoQuery : IQuery<BackupInfoResponse>;
