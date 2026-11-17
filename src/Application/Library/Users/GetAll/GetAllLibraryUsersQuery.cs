using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Users.GetAll;

public sealed record GetAllLibraryUsersQuery() : IQuery<List<LibraryUserResponse>>;


