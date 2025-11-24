using Application.Abstractions.Messaging;
using Application.Library.Settings.GetAllRoles;

namespace Application.Library.Settings.CreateRole;

public sealed record CreateRoleCommand(string Name) : ICommand<RoleResponse>;

