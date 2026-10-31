using Application.Abstractions.Messaging;

namespace Application.Library.Settings.GetSystemInfo;

public sealed record GetSystemInfoQuery() : IQuery<SystemInfoResponse>;


