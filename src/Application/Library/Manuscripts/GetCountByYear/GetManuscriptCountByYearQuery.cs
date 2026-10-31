using Application.Abstractions.Messaging;
using Application.Library.Shared;

namespace Application.Library.Manuscripts.GetCountByYear;

public sealed record GetManuscriptCountByYearQuery : IQuery<List<ListItemResponse>>;

