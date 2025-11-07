using Application.Abstractions.Messaging;

namespace Application.Library.Books.GetVolumeCount;

public sealed record GetBookVolumeCountQuery : IQuery<int>;

