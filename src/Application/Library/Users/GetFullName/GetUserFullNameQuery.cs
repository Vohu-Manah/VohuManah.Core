using Application.Abstractions.Messaging;

namespace Application.Library.Users.GetFullName;

public sealed record GetUserFullNameQuery(string UserName) : IQuery<string?>;

