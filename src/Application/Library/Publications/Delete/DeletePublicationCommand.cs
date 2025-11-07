using Application.Abstractions.Messaging;

namespace Application.Library.Publications.Delete;

public sealed record DeletePublicationCommand(int Id) : ICommand;

