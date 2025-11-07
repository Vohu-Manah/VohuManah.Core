using Application.Abstractions.Messaging;

namespace Application.Library.PublicationTypes.Delete;

public sealed record DeletePublicationTypeCommand(int Id) : ICommand;

