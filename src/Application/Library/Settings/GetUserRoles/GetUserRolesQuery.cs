using Application.Abstractions.Messaging;

namespace Application.Library.Settings.GetUserRoles;

public sealed record GetUserRolesQuery(long UserId) : IQuery<List<UserRoleResponse>>;

