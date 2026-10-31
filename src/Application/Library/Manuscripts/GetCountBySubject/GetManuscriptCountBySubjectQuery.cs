using Application.Abstractions.Messaging;
using Application.Library.Shared;

namespace Application.Library.Manuscripts.GetCountBySubject;

public sealed record GetManuscriptCountBySubjectQuery : IQuery<List<ListItemResponse>>;

