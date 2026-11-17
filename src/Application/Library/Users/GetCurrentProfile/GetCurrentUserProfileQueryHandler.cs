using VohuManah.Application.Abstractions.Data;
using VohuManah.Application.Abstractions.Messaging;
using VohuManah.Domain.Users;
using Microsoft.EntityFrameworkCore;
using VohuManah.SharedKernel;

namespace VohuManah.Application.Library.Users.GetCurrentProfile;

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

        var roleIds = await unitOfWork.GetDbSet<UserRole>()
            .Where(ur => ur.UserId == query.UserId)
            .Select(ur => ur.RoleId)
            .ToListAsync(cancellationToken);

        var permissions = await unitOfWork.GetDbSet<RolePermission>()
            .Where(rp => roleIds.Contains(rp.RoleId))
            .Select(rp => rp.EndpointName)
            .Distinct()
            .OrderBy(ep => ep)
            .ToListAsync(cancellationToken);

        var response = new UserProfileResponse
        {
            Id = user.Id,
            UserName = user.UserName,
            Name = user.Name,
            LastName = user.LastName,
            FullName = $"{user.Name} {user.LastName}".Trim(),
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Permissions = permissions
        };

        return Result.Success(response);
    }
}

