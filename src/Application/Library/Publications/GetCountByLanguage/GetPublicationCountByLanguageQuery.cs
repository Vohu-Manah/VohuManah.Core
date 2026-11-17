using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;

namespace VohuManah.Application.Library.Publications.GetCountByLanguage;

public sealed record GetPublicationCountByLanguageQuery : IQuery<List<ListItemResponse>>;

