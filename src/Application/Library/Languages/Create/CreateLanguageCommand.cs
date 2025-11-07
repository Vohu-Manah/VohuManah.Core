using Application.Abstractions.Messaging;

namespace Application.Library.Languages.Create;

public sealed record CreateLanguageCommand(string Name, string Abbreviation) : ICommand<int>;


