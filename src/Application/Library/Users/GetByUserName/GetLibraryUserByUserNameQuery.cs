using Application.Abstractions.Messaging;

namespace Application.Library.Users.GetByUserName;

public sealed record GetLibraryUserByUserNameQuery(string UserName) : IQuery<LibraryUserResponse>;


