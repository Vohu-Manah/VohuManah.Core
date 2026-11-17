using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Settings.GetBackground;

public sealed record GetCurrentBackgroundImageNameQuery() : IQuery<string>;


