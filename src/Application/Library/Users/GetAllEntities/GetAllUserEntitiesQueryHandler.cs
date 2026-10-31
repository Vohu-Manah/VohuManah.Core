using Application.Abstractions.Caching;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Users.GetAllEntities;

internal sealed class GetAllUserEntitiesQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetAllUserEntitiesQuery, List<User>>
{
    private const string UserAllKey = "sdp.user.all";

    public Task<Result<List<User>>> Handle(GetAllUserEntitiesQuery query, CancellationToken cancellationToken)
    {
        DbSet<User> users = unitOfWork.GetDbSet<User>();

        List<User> allUsers = cacheManager.Retrieve(UserAllKey, () => users.ToList());

        return Task.FromResult(Result.Success(allUsers));
    }
}

