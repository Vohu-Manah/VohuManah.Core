using Application.Abstractions.Messaging;

namespace Application.Library.Settings.GetAppInfo;

public sealed record GetAppInfoQuery() : IQuery<AppInfoResponse>;


