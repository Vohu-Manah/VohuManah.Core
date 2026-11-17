using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Languages.GetAll;

public sealed record GetAllLanguagesQuery(bool AddAllItemInFirstRow = false) : IQuery<List<LanguageResponse>>;


