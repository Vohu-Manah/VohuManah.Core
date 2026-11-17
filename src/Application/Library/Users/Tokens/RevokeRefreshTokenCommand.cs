using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Users.Tokens;

public sealed record RevokeRefreshTokenCommand(string RefreshToken) : ICommand;


