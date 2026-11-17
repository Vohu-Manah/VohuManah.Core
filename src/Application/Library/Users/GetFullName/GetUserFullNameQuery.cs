using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Users.GetFullName;

public sealed record GetUserFullNameQuery(string UserName) : IQuery<string?>;

