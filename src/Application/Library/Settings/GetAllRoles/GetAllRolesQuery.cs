using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Settings.GetAllRoles;

public sealed record GetAllRolesQuery() : IQuery<List<RoleResponse>>;

