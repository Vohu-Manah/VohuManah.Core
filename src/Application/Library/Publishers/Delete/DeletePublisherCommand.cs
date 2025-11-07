using Application.Abstractions.Messaging;

namespace Application.Library.Publishers.Delete;

public sealed record DeletePublisherCommand(int Id) : ICommand;


