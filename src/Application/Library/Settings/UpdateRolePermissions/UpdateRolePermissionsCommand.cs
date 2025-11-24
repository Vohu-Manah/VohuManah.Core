using Application.Abstractions.Messaging;

namespace Application.Library.Settings.UpdateRolePermissions;

public sealed record UpdateRolePermissionsCommand(
    int RoleId,
    IReadOnlyCollection<string> EndpointNames) : ICommand;

