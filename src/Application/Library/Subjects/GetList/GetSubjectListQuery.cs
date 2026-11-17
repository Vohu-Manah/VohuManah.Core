using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Subjects.GetList;

public sealed record GetSubjectListQuery : IQuery<List<SubjectListResponse>>;

