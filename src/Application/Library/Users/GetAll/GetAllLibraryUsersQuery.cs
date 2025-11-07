using Application.Abstractions.Messaging;

namespace Application.Library.Users.GetAll;

public sealed record GetAllLibraryUsersQuery() : IQuery<List<LibraryUserResponse>>;


