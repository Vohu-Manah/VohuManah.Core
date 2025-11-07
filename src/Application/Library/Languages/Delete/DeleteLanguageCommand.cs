using Application.Abstractions.Messaging;

namespace Application.Library.Languages.Delete;

public sealed record DeleteLanguageCommand(int Id) : ICommand;


