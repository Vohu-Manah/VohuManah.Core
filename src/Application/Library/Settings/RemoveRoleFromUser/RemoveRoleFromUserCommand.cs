using Application.Abstractions.Messaging;

namespace Application.Library.Settings.RemoveRoleFromUser;

public sealed record RemoveRoleFromUserCommand(
    string UserName,
    int RoleId) : ICommand;

