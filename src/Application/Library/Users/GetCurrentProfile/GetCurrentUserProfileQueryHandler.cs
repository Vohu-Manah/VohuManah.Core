using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Library.Users.GetCurrentProfile;

internal sealed class GetCurrentUserProfileQueryHandler(
    IUnitOfWork unitOfWork) : IQueryHandler<GetCurrentUserProfileQuery, UserProfileResponse>
{
    public async Task<Result<UserProfileResponse>> Handle(GetCurrentUserProfileQuery query, CancellationToken cancellationToken)
    {
        var users = unitOfWork.GetDbSet<User>();

        var user = await users
            .FirstOrDefaultAsync(u => u.Id == query.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Failure<UserProfileResponse>(UserErrors.NotFound(query.UserId));
        }

        var response = new UserProfileResponse
        {
            Id = user.Id,
            UserName = user.UserName,
            Name = user.Name,
            LastName = user.LastName,
            FullName = $"{user.Name} {user.LastName}".Trim(),
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };

        return Result.Success(response);
    }
}

