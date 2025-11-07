using Application.Abstractions.Messaging;

namespace Application.Library.Subjects.GetAll;

public sealed record GetAllSubjectsQuery(bool AddAllItemInFirstRow = false) : IQuery<List<SubjectResponse>>;


