using Application.Abstractions.Messaging;

namespace Application.Library.Users.Create;

public sealed record CreateLibraryUserCommand(
    string UserName,
    string Password,
    string Name,
    string LastName) : ICommand<string>;


