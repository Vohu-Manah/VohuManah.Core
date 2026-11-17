using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;

namespace VohuManah.Application.Library.Books.GetCountBySubject;

public sealed record GetBookCountBySubjectQuery : IQuery<List<ListItemResponse>>;

