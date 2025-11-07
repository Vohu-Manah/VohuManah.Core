using Application.Abstractions.Messaging;
using Application.Library._Shared;

namespace Application.Library.Books.GetCountByYear;

public sealed record GetBookCountByYearQuery : IQuery<List<ListItemResponse>>;

