using VohuManah.Application.Abstractions.Messaging;

namespace VohuManah.Application.Library.Users.GetCurrentProfile;

public sealed record GetCurrentUserProfileQuery(long UserId) : IQuery<UserProfileResponse>;

