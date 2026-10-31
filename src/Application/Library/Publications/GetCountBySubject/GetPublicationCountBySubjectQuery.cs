using Application.Abstractions.Messaging;
using Application.Library.Shared;

namespace Application.Library.Publications.GetCountBySubject;

public sealed record GetPublicationCountBySubjectQuery : IQuery<List<ListItemResponse>>;

