using Application.Abstractions.Messaging;

namespace Application.Library.Languages.GetAll;

public sealed record GetAllLanguagesQuery(bool AddAllItemInFirstRow = false) : IQuery<List<LanguageResponse>>;


