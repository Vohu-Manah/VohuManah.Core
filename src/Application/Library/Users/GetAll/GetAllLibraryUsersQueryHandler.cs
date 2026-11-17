using VohuManah.Application.Abstractions.Caching;
using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Users.GetAll;

internal sealed class GetAllLibraryUsersQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetAllLibraryUsersQuery, List<LibraryUserResponse>>
{
    private const string UserAllKey = "sdp.user.all";

    public Task<Result<List<LibraryUserResponse>>> Handle(GetAllLibraryUsersQuery query, CancellationToken cancellationToken)
    {
        DbSet<VohuManah.Domain.Users.User> users = unitOfWork.GetDbSet<VohuManah.Domain.Users.User>();

        List<VohuManah.Domain.Users.User> allUsers = cacheManager.Retrieve(UserAllKey, () => users.ToList());

        List<LibraryUserResponse> response = allUsers.Select(u => new LibraryUserResponse
        {
            Id = u.Id,
            UserName = u.UserName,
            Name = u.Name,
            LastName = u.LastName,
            FullName = $"{u.Name} {u.LastName}"
        }).ToList();

        return Task.FromResult(Result.Success(response));
    }
}


