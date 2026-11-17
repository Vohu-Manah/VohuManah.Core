using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Users;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Users.GetAllEntities;

internal sealed class GetAllUserEntitiesQueryHandler(
    IApplicationDbContext dbContext,
    ICacheManager cacheManager) : IQueryHandler<GetAllUserEntitiesQuery, List<User>>
{
    private const string UserAllKey = "sdp.user.all";

    public Task<Result<List<User>>> Handle(GetAllUserEntitiesQuery query, CancellationToken cancellationToken)
    {
        DbSet<User> users = dbContext.LibraryUsers;

        List<User> allUsers = cacheManager.Retrieve(UserAllKey, () => users.ToList());

        return Task.FromResult(Result.Success(allUsers));
    }
}

