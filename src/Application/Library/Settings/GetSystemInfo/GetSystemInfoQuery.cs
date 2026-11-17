using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Settings.GetSystemInfo;

public sealed record GetSystemInfoQuery() : IQuery<SystemInfoResponse>;


