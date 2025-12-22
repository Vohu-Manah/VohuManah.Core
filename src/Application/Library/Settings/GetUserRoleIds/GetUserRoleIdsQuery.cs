using Application.Abstractions.Messaging;

namespace Application.Library.Settings.GetUserRoleIds;

public sealed record GetUserRoleIdsQuery(long UserId) : IQuery<List<int>>;

