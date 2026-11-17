using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;

namespace VohuManah.Application.Library.Languages.GetNames;

public sealed record GetLanguageNamesQuery(bool AddAllItemInFirstRow = false) : IQuery<List<SelectItemResponse>>;


