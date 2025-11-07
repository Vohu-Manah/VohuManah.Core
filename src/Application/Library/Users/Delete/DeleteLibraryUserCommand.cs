using Application.Abstractions.Messaging;

namespace Application.Library.Users.Delete;

public sealed record DeleteLibraryUserCommand(string UserName) : ICommand;


