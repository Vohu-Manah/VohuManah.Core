using Application.Abstractions.Messaging;

namespace Application.Library.Users.Update;

public sealed record UpdateLibraryUserCommand(
    string UserName,
    string Password,
    string Name,
    string LastName) : ICommand;


