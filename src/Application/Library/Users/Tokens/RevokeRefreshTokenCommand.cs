using Application.Abstractions.Messaging;

namespace Application.Library.Users.Tokens;

public sealed record RevokeRefreshTokenCommand(string RefreshToken) : ICommand;


