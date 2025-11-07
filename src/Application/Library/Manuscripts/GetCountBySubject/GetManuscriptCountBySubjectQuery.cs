using Application.Abstractions.Messaging;
using Application.Library._Shared;

namespace Application.Library.Manuscripts.GetCountBySubject;

public sealed record GetManuscriptCountBySubjectQuery : IQuery<List<ListItemResponse>>;

