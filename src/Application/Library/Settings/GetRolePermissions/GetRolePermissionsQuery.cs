using Application.Abstractions.Messaging;

namespace Application.Library.Settings.GetRolePermissions;

public sealed record GetRolePermissionsQuery(int RoleId) : IQuery<List<string>>;

