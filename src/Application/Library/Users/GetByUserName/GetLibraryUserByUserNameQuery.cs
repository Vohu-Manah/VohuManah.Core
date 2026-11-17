using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Users.GetByUserName;

public sealed record GetLibraryUserByUserNameQuery(string UserName) : IQuery<LibraryUserResponse>;


