using Application.Abstractions.Messaging;

namespace Application.Library.Users.GetCurrentProfile;

public sealed record GetCurrentUserProfileQuery(long UserId) : IQuery<UserProfileResponse>;

