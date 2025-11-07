using Application.Abstractions.Messaging;

namespace Application.Library.Languages.GetList;

public sealed record GetLanguageListQuery : IQuery<List<LanguageListResponse>>;

