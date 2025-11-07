using Domain.Library;

namespace Application.Abstractions.Authentication;

public interface ILibraryTokenProvider
{
    Task<string> CreateAsync(User user, CancellationToken ct);
}


