using Application.Abstractions.Messaging;

namespace Application.Library.Subjects.GetList;

public sealed record GetSubjectListQuery : IQuery<List<SubjectListResponse>>;

