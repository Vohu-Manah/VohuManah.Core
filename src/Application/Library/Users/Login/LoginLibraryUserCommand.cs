using Application.Abstractions.Messaging;

namespace Application.Library.Users.Login;

public sealed record LoginLibraryUserCommand(
    string UserName,
    string Password) : ICommand<LoginLibraryUserResponse>;


