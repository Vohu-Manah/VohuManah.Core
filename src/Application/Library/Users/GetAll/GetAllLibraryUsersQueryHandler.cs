using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Application.Abstractions.Caching;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Users.GetAll;

internal sealed class GetAllLibraryUsersQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetAllLibraryUsersQuery, List<LibraryUserResponse>>
{
    private const string UserAllKey = "sdp.user.all";

    public Task<Result<List<LibraryUserResponse>>> Handle(GetAllLibraryUsersQuery query, CancellationToken cancellationToken)
    {
        DbSet<Domain.Library.User> users = unitOfWork.Set<Domain.Library.User>();
        
        List<Domain.Library.User> allUsers = cacheManager.Get(UserAllKey, () => users.ToList());
        
        List<LibraryUserResponse> response = allUsers.Select(u => new LibraryUserResponse
        {
            UserName = u.UserName,
            Name = u.Name,
            LastName = u.LastName,
            FullName = $"{u.Name} {u.LastName}"
        }).ToList();

        return Task.FromResult(Result.Success(response));
    }
}


