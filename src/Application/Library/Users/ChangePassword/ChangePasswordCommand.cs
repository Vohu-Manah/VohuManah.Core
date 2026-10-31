using Application.Abstractions.Messaging;

namespace Application.Library.Users.ChangePassword;

public sealed record ChangePasswordCommand(
    long UserId,
    string CurrentPassword,
    string NewPassword) : ICommand;

