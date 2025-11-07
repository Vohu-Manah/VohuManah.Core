using Application.Abstractions.Messaging;

namespace Application.Library.Subjects.Create;

public sealed record CreateSubjectCommand(string Title) : ICommand<int>;


