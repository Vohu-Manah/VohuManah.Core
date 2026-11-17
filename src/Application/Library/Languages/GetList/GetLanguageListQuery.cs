using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Languages.GetList;

public sealed record GetLanguageListQuery : IQuery<List<LanguageListResponse>>;

