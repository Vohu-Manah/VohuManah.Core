using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Users;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Users.GetByUserName;

internal sealed class GetLibraryUserByUserNameQueryHandler(
    IApplicationDbContext dbContext,
    ICacheManager cacheManager) : IQueryHandler<GetLibraryUserByUserNameQuery, LibraryUserResponse>
{
    private const string UserAllKey = "sdp.user.all";

    public Task<Result<LibraryUserResponse>> Handle(GetLibraryUserByUserNameQuery query, CancellationToken cancellationToken)
    {
        DbSet<VohuManah.Domain.Users.User> users = dbContext.LibraryUsers;

        List<VohuManah.Domain.Users.User> allUsers = cacheManager.Retrieve(UserAllKey, () => users.ToList());

        VohuManah.Domain.Users.User? user = allUsers.SingleOrDefault(u => u.UserName == query.UserName);

        if (user == null)
        {
            return Task.FromResult(Result.Failure<LibraryUserResponse>(UserErrors.NotFound(query.UserName)));
        }

        LibraryUserResponse response = new LibraryUserResponse
        {
            Id = user.Id,
            UserName = user.UserName,
            Name = user.Name,
            LastName = user.LastName,
            FullName = $"{user.Name} {user.LastName}"
        };

        return Task.FromResult(Result.Success(response));
    }
}
