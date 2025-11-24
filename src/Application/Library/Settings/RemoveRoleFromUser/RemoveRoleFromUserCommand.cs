using Application.Abstractions.Messaging;

namespace Application.Library.Settings.RemoveRoleFromUser;

public sealed record RemoveRoleFromUserCommand(
    long UserId,
    int RoleId) : ICommand;

