using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Subjects.GetAll;

public sealed record GetAllSubjectsQuery(bool AddAllItemInFirstRow = false) : IQuery<List<SubjectResponse>>;


