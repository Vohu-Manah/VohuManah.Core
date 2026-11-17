using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;

namespace VohuManah.Application.Library.Manuscripts.GetCountByLanguage;

public sealed record GetManuscriptCountByLanguageQuery : IQuery<List<ListItemResponse>>;

