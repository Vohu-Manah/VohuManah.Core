using Application.Abstractions.Messaging;

namespace Application.Library.Settings.AssignRoleToUser;

public sealed record AssignRoleToUserCommand(
    long UserId,
    int RoleId) : ICommand;

