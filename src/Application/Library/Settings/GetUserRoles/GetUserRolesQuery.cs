using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Settings.GetUserRoles;

public sealed record GetUserRolesQuery(long UserId) : IQuery<List<UserRoleResponse>>;

