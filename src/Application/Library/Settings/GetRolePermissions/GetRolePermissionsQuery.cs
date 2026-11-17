using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Settings.GetRolePermissions;

public sealed record GetRolePermissionsQuery(int RoleId) : IQuery<List<string>>;

