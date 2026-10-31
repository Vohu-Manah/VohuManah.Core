using Application.Abstractions.Messaging;
using Application.Library.Shared;

namespace Application.Library.Subjects.GetNames;

public sealed record GetSubjectNamesQuery(bool AddAllItemInFirstRow = false) : IQuery<List<SelectItemResponse>>;


