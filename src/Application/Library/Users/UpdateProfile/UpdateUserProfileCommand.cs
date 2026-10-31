using Application.Abstractions.Messaging;

namespace Application.Library.Users.UpdateProfile;

public sealed record UpdateUserProfileCommand(
    long UserId,
    string Name,
    string LastName,
    string? Email,
    string? PhoneNumber) : ICommand;

