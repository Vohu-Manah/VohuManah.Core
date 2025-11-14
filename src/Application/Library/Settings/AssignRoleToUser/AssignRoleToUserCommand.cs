using Application.Abstractions.Messaging;

namespace Application.Library.Settings.AssignRoleToUser;

public sealed record AssignRoleToUserCommand(
    string UserName,
    int RoleId) : ICommand;

