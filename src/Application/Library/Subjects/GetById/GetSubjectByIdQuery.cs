using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Subjects.GetById;

public sealed record GetSubjectByIdQuery(int Id) : IQuery<SubjectResponse>;

