using Application.Abstractions.Messaging;

namespace Application.Library.Settings.GetBackground;

public sealed record GetCurrentBackgroundImageFolderQuery() : IQuery<string>;


