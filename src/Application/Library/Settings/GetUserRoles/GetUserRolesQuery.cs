using Application.Abstractions.Messaging;

namespace Application.Library.Settings.GetUserRoles;

public sealed record GetUserRolesQuery(string UserName) : IQuery<List<UserRoleResponse>>;

