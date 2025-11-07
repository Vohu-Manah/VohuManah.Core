using Application.Abstractions.Messaging;

namespace Application.Library.Subjects.Delete;

public sealed record DeleteSubjectCommand(int Id) : ICommand;


