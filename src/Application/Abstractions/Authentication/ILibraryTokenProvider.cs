using VohuManah.Domain.Users;

namespace VohuManah.Application.Abstractions.Authentication;

public interface ILibraryTokenProvider
{
    Task<string> CreateAsync(User user, CancellationToken ct);
}


