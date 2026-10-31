using Application.Abstractions.Messaging;
using Application.Library.Shared;

namespace Application.Library.Manuscripts.GetCountByGap;

public sealed record GetManuscriptCountByGapQuery : IQuery<List<ListItemResponse>>;

