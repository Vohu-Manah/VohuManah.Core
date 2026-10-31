using Application.Abstractions.Messaging;
using Application.Library.Shared;

namespace Application.Library.Manuscripts.GetCountByWritingPlace;

public sealed record GetManuscriptCountByWritingPlaceQuery : IQuery<List<ListItemResponse>>;

