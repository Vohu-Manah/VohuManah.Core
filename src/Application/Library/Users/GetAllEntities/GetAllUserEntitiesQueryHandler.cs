using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Caching;
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
        var users = unitOfWork.Set<User>();
        
        var allUsers = cacheManager.Get(UserAllKey, () => users.ToList());

        return Task.FromResult(Result.Success(allUsers));
    }
}

