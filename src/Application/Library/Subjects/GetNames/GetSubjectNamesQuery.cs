using Application.Library._Shared;
using Application.Abstractions.Messaging;

namespace Application.Library.Subjects.GetNames;

public sealed record GetSubjectNamesQuery(bool AddAllItemInFirstRow = false) : IQuery<List<SelectItemResponse>>;


