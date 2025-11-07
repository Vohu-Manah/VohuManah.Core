using Application.Abstractions.Messaging;
using Application.Library._Shared;

namespace Application.Library.Publications.GetCountBySubject;

public sealed record GetPublicationCountBySubjectQuery : IQuery<List<ListItemResponse>>;

