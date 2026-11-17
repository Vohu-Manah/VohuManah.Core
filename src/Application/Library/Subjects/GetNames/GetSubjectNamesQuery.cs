using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Application.Library.Shared;

namespace VohuManah.Application.Library.Subjects.GetNames;

public sealed record GetSubjectNamesQuery(bool AddAllItemInFirstRow = false) : IQuery<List<SelectItemResponse>>;


