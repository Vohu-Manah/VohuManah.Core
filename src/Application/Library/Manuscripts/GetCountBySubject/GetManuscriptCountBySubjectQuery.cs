using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;

namespace VohuManah.Application.Library.Manuscripts.GetCountBySubject;

public sealed record GetManuscriptCountBySubjectQuery : IQuery<List<ListItemResponse>>;

