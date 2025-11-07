using Application.Abstractions.Messaging;

namespace Application.Library.PublicationTypes.Update;

public sealed record UpdatePublicationTypeCommand(int Id, string Title) : ICommand;

