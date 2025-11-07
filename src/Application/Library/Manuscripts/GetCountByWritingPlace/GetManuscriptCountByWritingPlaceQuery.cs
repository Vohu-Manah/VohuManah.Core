using Application.Abstractions.Messaging;
using Application.Library._Shared;

namespace Application.Library.Manuscripts.GetCountByWritingPlace;

public sealed record GetManuscriptCountByWritingPlaceQuery : IQuery<List<ListItemResponse>>;

