using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Library;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Users.GetByUserName;

internal sealed class GetLibraryUserByUserNameQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetLibraryUserByUserNameQuery, LibraryUserResponse>
{
    private const string UserAllKey = "sdp.user.all";

    public Task<Result<LibraryUserResponse>> Handle(GetLibraryUserByUserNameQuery query, CancellationToken cancellationToken)
    {
        DbSet<VohuManah.Domain.Library.User> users = unitOfWork.GetDbSet<VohuManah.Domain.Library.User>();

        List<VohuManah.Domain.Library.User> allUsers = cacheManager.Retrieve(UserAllKey, () => users.ToList());

        VohuManah.Domain.Library.User? user = allUsers.SingleOrDefault(u => u.UserName == query.UserName);

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
