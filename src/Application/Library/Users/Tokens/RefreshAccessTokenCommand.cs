using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Users.Tokens;

public sealed record RefreshAccessTokenCommand(string RefreshToken) : ICommand<RefreshAccessTokenResponse>;
