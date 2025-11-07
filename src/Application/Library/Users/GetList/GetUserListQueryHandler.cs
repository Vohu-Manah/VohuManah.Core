using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Caching;
using Domain.Library;
using SharedKernel;

namespace Application.Library.Users.GetList;

internal sealed class GetUserListQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetUserListQuery, List<UserListResponse>>
{
    private const string UserAllKey = "sdp.user.all";

    public Task<Result<List<UserListResponse>>> Handle(GetUserListQuery query, CancellationToken cancellationToken)
    {
        var users = unitOfWork.Set<User>();
        
        var allUsers = cacheManager.Get(UserAllKey, () => users.ToList());
        
        var list = allUsers.Select(u => new UserListResponse
        {
            UserName = u.UserName,
            FullName = $"{u.Name} {u.LastName}"
        }).ToList();

        return Task.FromResult(Result.Success(list));
    }
}

