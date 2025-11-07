using Application.Abstractions.Messaging;

namespace Application.Library.Manuscripts.Delete;

public sealed record DeleteManuscriptCommand(int Id) : ICommand;

