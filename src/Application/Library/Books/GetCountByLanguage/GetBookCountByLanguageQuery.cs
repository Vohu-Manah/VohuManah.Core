using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;

namespace VohuManah.Application.Library.Books.GetCountByLanguage;

public sealed record GetBookCountByLanguageQuery : IQuery<List<ListItemResponse>>;

