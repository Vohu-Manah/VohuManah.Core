using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Users.GetFullName;

internal sealed class GetUserFullNameQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetUserFullNameQuery, string?>
{
    private const string UserAllKey = "sdp.user.all";

    public Task<Result<string?>> Handle(GetUserFullNameQuery query, CancellationToken cancellationToken)
    {
        var users = unitOfWork.GetDbSet<User>();

        var allUsers = cacheManager.Retrieve(UserAllKey, () => users.ToList());

        var fullName = allUsers
            .Where(u => u.UserName == query.UserName)
            .Select(u => $"{u.Name} {u.LastName}")
            .SingleOrDefault();

        return Task.FromResult(Result.Success(fullName));
    }
}

