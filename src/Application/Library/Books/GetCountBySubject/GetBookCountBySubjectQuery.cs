using Application.Abstractions.Messaging;
using Application.Library._Shared;

namespace Application.Library.Books.GetCountBySubject;

public sealed record GetBookCountBySubjectQuery : IQuery<List<ListItemResponse>>;

