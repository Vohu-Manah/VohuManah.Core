using Application.Abstractions.Messaging;

namespace Application.Library.Subjects.GetById;

public sealed record GetSubjectByIdQuery(int Id) : IQuery<SubjectResponse>;

