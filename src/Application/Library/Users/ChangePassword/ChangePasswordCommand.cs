using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Users.ChangePassword;

public sealed record ChangePasswordCommand(
    long UserId,
    string CurrentPassword,
    string NewPassword) : ICommand;

