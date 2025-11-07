using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Caching;
using Domain.Library;
using SharedKernel;

namespace Application.Library.Users.GetFullName;

internal sealed class GetUserFullNameQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetUserFullNameQuery, string?>
{
    private const string UserAllKey = "sdp.user.all";

    public Task<Result<string?>> Handle(GetUserFullNameQuery query, CancellationToken cancellationToken)
    {
        var users = unitOfWork.Set<User>();
        
        var allUsers = cacheManager.Get(UserAllKey, () => users.ToList());
        
        var fullName = allUsers
            .Where(u => u.UserName == query.UserName)
            .Select(u => $"{u.Name} {u.LastName}")
            .SingleOrDefault();

        return Task.FromResult(Result.Success(fullName));
    }
}

