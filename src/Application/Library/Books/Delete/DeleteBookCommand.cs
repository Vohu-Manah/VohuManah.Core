using Application.Abstractions.Messaging;

namespace Application.Library.Books.Delete;

public sealed record DeleteBookCommand(long Id) : ICommand;

