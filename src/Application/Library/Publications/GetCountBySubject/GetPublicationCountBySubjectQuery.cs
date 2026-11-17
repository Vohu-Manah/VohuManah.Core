using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;

namespace VohuManah.Application.Library.Publications.GetCountBySubject;

public sealed record GetPublicationCountBySubjectQuery : IQuery<List<ListItemResponse>>;

