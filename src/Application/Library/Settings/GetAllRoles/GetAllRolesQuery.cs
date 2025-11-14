using Application.Abstractions.Messaging;

namespace Application.Library.Settings.GetAllRoles;

public sealed record GetAllRolesQuery() : IQuery<List<RoleResponse>>;

