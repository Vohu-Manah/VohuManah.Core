using Application.Abstractions.Messaging;
using Application.Library.Shared;

namespace Application.Library.Books.GetCountByYear;

public sealed record GetBookCountByYearQuery : IQuery<List<ListItemResponse>>;

