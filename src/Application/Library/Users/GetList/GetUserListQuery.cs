using Application.Abstractions.Messaging;

namespace Application.Library.Users.GetList;

public sealed record GetUserListQuery : IQuery<List<UserListResponse>>;

