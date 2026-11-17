using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Users.GetList;

public sealed record GetUserListQuery : IQuery<List<UserListResponse>>;

