using Application.Abstractions.Messaging;

namespace Application.Library.Languages.Update;

public sealed record UpdateLanguageCommand(int Id, string Name, string Abbreviation) : ICommand;


