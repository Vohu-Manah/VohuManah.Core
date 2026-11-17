using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Users.GetList;

internal sealed class GetUserListQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetUserListQuery, List<UserListResponse>>
{
    private const string UserAllKey = "sdp.user.all";

    public Task<Result<List<UserListResponse>>> Handle(GetUserListQuery query, CancellationToken cancellationToken)
    {
        var users = unitOfWork.GetDbSet<User>();

        var allUsers = cacheManager.Retrieve(UserAllKey, () => users.ToList());

        var list = allUsers.Select(u => new UserListResponse
        {
            Id = u.Id,
            UserName = u.UserName,
            Name = u.Name,
            LastName = u.LastName,
            FullName = $"{u.Name} {u.LastName}"
        }).ToList();

        return Task.FromResult(Result.Success(list));
    }
}

