using Application.Abstractions.Messaging;

namespace Application.Library.Settings.GetBackground;

public sealed record GetCurrentBackgroundImageNameQuery() : IQuery<string>;


