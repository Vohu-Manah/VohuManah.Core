using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Settings.GetUserRoleIds;

public sealed record GetUserRoleIdsQuery(long UserId) : IQuery<List<int>>;

