using Application.Abstractions.Messaging;

namespace Application.Library.Subjects.Update;

public sealed record UpdateSubjectCommand(int Id, string Title) : ICommand;


