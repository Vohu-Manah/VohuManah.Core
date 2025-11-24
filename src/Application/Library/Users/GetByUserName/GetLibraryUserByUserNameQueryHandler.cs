using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Application.Abstractions.Caching;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Users.GetByUserName;

internal sealed class GetLibraryUserByUserNameQueryHandler(
    IUnitOfWork unitOfWork,
    ICacheManager cacheManager) : IQueryHandler<GetLibraryUserByUserNameQuery, LibraryUserResponse>
{
    private const string UserAllKey = "sdp.user.all";

    public Task<Result<LibraryUserResponse>> Handle(GetLibraryUserByUserNameQuery query, CancellationToken cancellationToken)
    {
        DbSet<Domain.Library.User> users = unitOfWork.Set<Domain.Library.User>();
        
        List<Domain.Library.User> allUsers = cacheManager.Get(UserAllKey, () => users.ToList());
        
        Domain.Library.User? user = allUsers.SingleOrDefault(u => u.UserName == query.UserName);
        
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
