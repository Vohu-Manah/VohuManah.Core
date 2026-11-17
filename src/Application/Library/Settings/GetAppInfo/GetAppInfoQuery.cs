using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Settings.GetAppInfo;

public sealed record GetAppInfoQuery() : IQuery<AppInfoResponse>;


