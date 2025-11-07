using Application.Abstractions.Messaging;
using Application.Library._Shared;

namespace Application.Library.Manuscripts.GetCountByGap;

public sealed record GetManuscriptCountByGapQuery : IQuery<List<ListItemResponse>>;

