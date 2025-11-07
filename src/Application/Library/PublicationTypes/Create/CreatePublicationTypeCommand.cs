using Application.Abstractions.Messaging;

namespace Application.Library.PublicationTypes.Create;

public sealed record CreatePublicationTypeCommand(string Title) : ICommand<int>;

