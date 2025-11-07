using Application.Abstractions.Messaging;

namespace Application.Library.Users.Tokens;

public sealed record RefreshAccessTokenCommand(string RefreshToken) : ICommand<string>;


